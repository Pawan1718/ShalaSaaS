using System.Data;
using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Domain.Enums;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public class FeeChargeGenerationService : IFeeChargeGenerationService
{
    private readonly IStudentFeeAssignmentRepository _assignmentRepository;
    private readonly IFeeStructureRepository _structureRepository;
    private readonly IStudentChargeRepository _chargeRepository;
    private readonly IFeeLedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public FeeChargeGenerationService(
        IStudentFeeAssignmentRepository assignmentRepository,
        IFeeStructureRepository structureRepository,
        IStudentChargeRepository chargeRepository,
        IFeeLedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _assignmentRepository = assignmentRepository;
        _structureRepository = structureRepository;
        _chargeRepository = chargeRepository;
        _ledgerPostingService = ledgerPostingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, string Message, List<StudentCharge> Charges)> GenerateAsync(
        int tenantId,
        int branchId,
        int studentFeeAssignmentId,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken, IsolationLevel.Serializable);

        try
        {
            var assignment = await _assignmentRepository.GetByIdAsync(
                studentFeeAssignmentId,
                tenantId,
                branchId,
                cancellationToken);

            if (assignment is null)
                return await RollbackAsync("Student fee assignment not found.", cancellationToken);

            if (!assignment.IsActive)
                return await RollbackAsync("Inactive assignment cannot generate charges.", cancellationToken);

            var structure = await _structureRepository.GetWithItemsAsync(
                assignment.FeeStructureId,
                tenantId,
                branchId,
                cancellationToken);

            if (structure is null)
                return await RollbackAsync("Fee structure not found.", cancellationToken);

            if (structure.Items == null || structure.Items.Count == 0)
                return await RollbackAsync("Fee structure has no items.", cancellationToken);

            var existingCharges = await _chargeRepository.GetByAssignmentIdAsync(
                studentFeeAssignmentId,
                tenantId,
                branchId,
                cancellationToken);

            if (existingCharges.Any(x => x.PaidAmount > 0))
                return await RollbackAsync("Cannot regenerate charges because some charges are already paid.", cancellationToken);

            foreach (var existingCharge in existingCharges.Where(x => !x.IsCancelled))
            {
                existingCharge.IsCancelled = true;
                existingCharge.IsSettled = false;
                _chargeRepository.Update(existingCharge);
            }

            var charges = new List<StudentCharge>();
            var academicYear = structure.AcademicYearId;

            foreach (var item in structure.Items.Where(x => x.IsActive && !x.IsOptional))
            {
                if (item.Amount <= 0)
                    continue;

                if (!await ShouldGenerateItemAsync(item, assignment, academicYear, tenantId, branchId, cancellationToken))
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
                return await RollbackAsync("No valid charges could be generated.", cancellationToken);

            DistributeDiscount(charges, assignment.DiscountAmount);
            DistributeAdditionalAmount(charges, assignment.AdditionalChargeAmount);

            await _chargeRepository.AddRangeAsync(charges, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var ledgerChargePayload = existingCharges
                .Concat(charges)
                .Select(ToChargeResponse)
                .ToList();

            await _ledgerPostingService.PostChargesAsync(
                tenantId,
                branchId,
                assignment.StudentId,
                assignment.StudentAdmissionId,
                ledgerChargePayload,
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return (true, "Student charges generated successfully.", charges);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<bool> ShouldGenerateItemAsync(
        FeeStructureItem item,
        StudentFeeAssignment assignment,
        int academicYear,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken)
    {
        if (assignment.StudentId <= 0)
            return false;

        var applyType = (FeeApplyType)item.ApplyType;

        return applyType switch
        {
            FeeApplyType.FirstAdmissionOnly =>
                await _assignmentRepository.IsFirstAdmissionForStudentAsync(
                    assignment.StudentId,
                    assignment.StudentAdmissionId,
                    tenantId,
                    branchId,
                    cancellationToken)
                && !await _chargeRepository.ExistsHistoricalChargeForFeeHeadAsync(
                    assignment.StudentId,
                    item.FeeHeadId,
                    tenantId,
                    branchId,
                    assignment.Id,
                    cancellationToken),

            FeeApplyType.OneTime =>
                !await _chargeRepository.ExistsAnyHistoricalChargeForFeeHeadAsync(
                    assignment.StudentId,
                    item.FeeHeadId,
                    tenantId,
                    branchId,
                    assignment.Id,
                    cancellationToken),

            FeeApplyType.EveryYear =>
                !await _chargeRepository.ExistsChargeForFeeHeadInAcademicYearAsync(
                    assignment.StudentId,
                    item.FeeHeadId,
                    academicYear,
                    tenantId,
                    branchId,
                    assignment.Id,
                    cancellationToken),

            _ => true
        };
    }

    private static StudentChargeResponse ToChargeResponse(StudentCharge charge)
    {
        return new StudentChargeResponse
        {
            Id = charge.Id,
            FeeHeadId = charge.FeeHeadId,
            Amount = charge.Amount,
            DiscountAmount = charge.DiscountAmount,
            FineAmount = charge.FineAmount,
            PaidAmount = charge.PaidAmount,
            DueDate = charge.DueDate,
            IsCancelled = charge.IsCancelled
        };
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
            DiscountAmount = 0m,
            FineAmount = 0m,
            PaidAmount = 0m,
            IsSettled = false,
            IsCancelled = false
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

    private static string GetMonthName(int month)
        => new DateTime(2000, NormalizeMonth(month), 1).ToString("MMM");

    private async Task<(bool Success, string Message, List<StudentCharge> Charges)> RollbackAsync(
        string message,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        return (false, message, new List<StudentCharge>());
    }
}