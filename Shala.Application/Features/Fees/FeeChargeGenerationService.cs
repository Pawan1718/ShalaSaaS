using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Domain.Enums;

namespace Shala.Application.Features.Fees;

public class FeeChargeGenerationService : IFeeChargeGenerationService
{
    private readonly IStudentFeeAssignmentRepository _assignmentRepository;
    private readonly IFeeStructureRepository _structureRepository;
    private readonly IStudentChargeRepository _chargeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FeeChargeGenerationService(
        IStudentFeeAssignmentRepository assignmentRepository,
        IFeeStructureRepository structureRepository,
        IStudentChargeRepository chargeRepository,
        IUnitOfWork unitOfWork)
    {
        _assignmentRepository = assignmentRepository;
        _structureRepository = structureRepository;
        _chargeRepository = chargeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, string Message, List<StudentCharge> Charges)> GenerateAsync(
        int tenantId,
        int branchId,
        int studentFeeAssignmentId,
        CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(
            studentFeeAssignmentId,
            tenantId,
            branchId,
            cancellationToken);

        if (assignment is null)
            return (false, "Student fee assignment not found.", new List<StudentCharge>());

        var structure = await _structureRepository.GetWithItemsAsync(
            assignment.FeeStructureId,
            tenantId,
            branchId,
            cancellationToken);

        if (structure is null)
            return (false, "Fee structure not found.", new List<StudentCharge>());

        if (structure.Items == null || structure.Items.Count == 0)
            return (false, "Fee structure has no items.", new List<StudentCharge>());

        var existingCharges = await _chargeRepository.GetByAssignmentIdAsync(
            studentFeeAssignmentId,
            tenantId,
            branchId,
            cancellationToken);

        var hasPaidCharges = existingCharges.Any(x => x.PaidAmount > 0);

        if (hasPaidCharges)
            return (false, "Cannot regenerate charges because some charges are already paid.", new List<StudentCharge>());

        if (existingCharges.Any())
        {
            // Assumes repository supports bulk delete.
            // If not, replace with per-item delete/update logic from your repository.
            await _chargeRepository.DeleteRangeAsync(existingCharges, cancellationToken);
        }

        var charges = new List<StudentCharge>();
        var academicYear = structure.AcademicYearId;

        foreach (var item in structure.Items.Where(x => x.IsActive && !x.IsOptional))
        {
            if (item.Amount <= 0)
                continue;

            if (!ShouldGenerateItem(item, assignment))
                continue;

            switch ((FeeFrequencyType)item.FrequencyType)
            {
                case FeeFrequencyType.OneTime:
                case FeeFrequencyType.Yearly:
                    charges.Add(CreateCharge(
                        tenantId,
                        branchId,
                        assignment,
                        item,
                        item.Label.Trim(),
                        structure.Name,
                        BuildDueDate(academicYear, item.StartMonth ?? 4, item.DueDay ?? 10),
                        item.Amount));
                    break;

                case FeeFrequencyType.Monthly:
                    foreach (var month in BuildMonthRange(item.StartMonth, item.EndMonth))
                    {
                        charges.Add(CreateCharge(
                            tenantId,
                            branchId,
                            assignment,
                            item,
                            $"{GetMonthName(month)} {item.Label.Trim()}",
                            $"{GetMonthName(month)}-{academicYear}",
                            BuildDueDate(academicYear, month, item.DueDay ?? 10),
                            item.Amount));
                    }
                    break;

                case FeeFrequencyType.Quarterly:
                    foreach (var month in BuildQuarterMonths(item.StartMonth, item.EndMonth))
                    {
                        charges.Add(CreateCharge(
                            tenantId,
                            branchId,
                            assignment,
                            item,
                            $"{GetMonthName(month)} Quarter {item.Label.Trim()}",
                            $"{GetMonthName(month)}-{academicYear}",
                            BuildDueDate(academicYear, month, item.DueDay ?? 10),
                            item.Amount));
                    }
                    break;

                default:
                    charges.Add(CreateCharge(
                        tenantId,
                        branchId,
                        assignment,
                        item,
                        item.Label.Trim(),
                        "Custom",
                        BuildDueDate(academicYear, item.StartMonth ?? 4, item.DueDay ?? 10),
                        item.Amount));
                    break;
            }
        }

        if (charges.Count == 0)
            return (false, "No valid charges could be generated.", new List<StudentCharge>());

        DistributeDiscount(charges, assignment.DiscountAmount);
        DistributeAdditionalAmount(charges, assignment.AdditionalChargeAmount);

        await _chargeRepository.AddRangeAsync(charges, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Student charges generated successfully.", charges);
    }

    private static StudentCharge CreateCharge(
        int tenantId,
        int branchId,
        StudentFeeAssignment assignment,
        FeeStructureItem item,
        string chargeLabel,
        string periodLabel,
        DateTime dueDate,
        decimal amount)
    {
        return new StudentCharge
        {
            TenantId = tenantId,
            BranchId = branchId,
            StudentId = assignment.StudentId,
            StudentAdmissionId = assignment.StudentAdmissionId,
            StudentFeeAssignmentId = assignment.Id,
            FeeHeadId = item.FeeHeadId,
            ChargeLabel = chargeLabel,
            PeriodLabel = periodLabel,
            DueDate = dueDate,
            Amount = amount,
            DiscountAmount = 0,
            FineAmount = 0,
            PaidAmount = 0,
            IsSettled = false,
            IsCancelled = false
        };
    }

    private static bool ShouldGenerateItem(FeeStructureItem item, StudentFeeAssignment assignment)
    {
        var applyType = (FeeApplyType)item.ApplyType;

        return applyType switch
        {
            FeeApplyType.EveryYear => true,

            // Adjust this logic if you have a proper "is first admission" field.
            FeeApplyType.FirstAdmissionOnly => true,

            FeeApplyType.OneTime => true,

            _ => true
        };
    }

    private static void DistributeDiscount(List<StudentCharge> charges, decimal totalDiscount)
    {
        if (totalDiscount <= 0 || charges.Count == 0)
            return;

        var count = charges.Count;
        var perCharge = Math.Floor((totalDiscount / count) * 100m) / 100m;
        var remainder = totalDiscount - (perCharge * count);

        foreach (var charge in charges)
            charge.DiscountAmount = perCharge;

        for (var i = 0; i < charges.Count && remainder > 0; i++)
        {
            var extra = Math.Min(0.01m, remainder);
            charges[i].DiscountAmount += extra;
            remainder -= extra;
        }
    }

    private static void DistributeAdditionalAmount(List<StudentCharge> charges, decimal totalAdditionalAmount)
    {
        if (totalAdditionalAmount <= 0 || charges.Count == 0)
            return;

        var count = charges.Count;
        var perCharge = Math.Floor((totalAdditionalAmount / count) * 100m) / 100m;
        var remainder = totalAdditionalAmount - (perCharge * count);

        foreach (var charge in charges)
            charge.Amount += perCharge;

        for (var i = 0; i < charges.Count && remainder > 0; i++)
        {
            var extra = Math.Min(0.01m, remainder);
            charges[i].Amount += extra;
            remainder -= extra;
        }
    }

    private static List<int> BuildMonthRange(int? startMonth, int? endMonth)
    {
        var start = NormalizeMonth(startMonth ?? 4);
        var end = NormalizeMonth(endMonth ?? 3);

        var months = new List<int>();
        var current = start;

        while (true)
        {
            months.Add(current);

            if (current == end)
                break;

            current++;
            if (current > 12)
                current = 1;
        }

        return months;
    }

    private static List<int> BuildQuarterMonths(int? startMonth, int? endMonth)
    {
        var allMonths = BuildMonthRange(startMonth, endMonth);
        var quarterMonths = new List<int>();

        for (var i = 0; i < allMonths.Count; i += 3)
            quarterMonths.Add(allMonths[i]);

        return quarterMonths;
    }

    private static int NormalizeMonth(int month)
    {
        if (month < 1) return 1;
        if (month > 12) return 12;
        return month;
    }

    private static DateTime BuildDueDate(int year, int month, int day)
    {
        var normalizedMonth = NormalizeMonth(month);
        var dueYear = normalizedMonth >= 4 ? year : year + 1;
        var safeDay = Math.Min(Math.Max(day, 1), DateTime.DaysInMonth(dueYear, normalizedMonth));

        return new DateTime(dueYear, normalizedMonth, safeDay);
    }

    private static string GetMonthName(int month) =>
        new DateTime(2000, NormalizeMonth(month), 1).ToString("MMM");
}