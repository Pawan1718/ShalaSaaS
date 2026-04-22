using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Infrastructure.Data;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Infrastructure.Repositories.Fees;

public sealed class FeeDashboardReadRepository : IFeeDashboardReadRepository
{
    private readonly AppDbContext _db;

    public FeeDashboardReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<FeeDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        FeeDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var admissionsQuery =
            from admission in _db.StudentAdmissions.AsNoTracking()
            join student in _db.Students.AsNoTracking() on admission.StudentId equals student.Id
            join academicYear in _db.AcademicYears.AsNoTracking() on admission.AcademicYearId equals academicYear.Id
            join academicClass in _db.AcademicClasses.AsNoTracking() on admission.AcademicClassId equals academicClass.Id
            join sectionLeft in _db.Sections.AsNoTracking() on admission.SectionId equals sectionLeft.Id into sectionJoin
            from section in sectionJoin.DefaultIfEmpty()
            where admission.TenantId == tenantId
                  && admission.BranchId == branchId
                  && admission.IsCurrent
            select new
            {
                AdmissionId = admission.Id,
                admission.StudentId,
                admission.AdmissionNo,
                admission.RollNo,
                admission.AcademicYearId,
                admission.AcademicClassId,
                admission.SectionId,
                StudentName =
                    ((student.FirstName ?? string.Empty) + " " +
                     (student.MiddleName ?? string.Empty) + " " +
                     (student.LastName ?? string.Empty)).Trim(),
                AcademicYearName = academicYear.Name,
                ClassName = academicClass.Name,
                SectionName = section != null ? section.Name : null
            };

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim().ToLower();

            admissionsQuery = admissionsQuery.Where(x =>
                x.StudentName.ToLower().Contains(search) ||
                x.AdmissionNo.ToLower().Contains(search) ||
                (x.RollNo != null && x.RollNo.ToLower().Contains(search)));
        }

        if (request.AcademicYearId.HasValue)
            admissionsQuery = admissionsQuery.Where(x => x.AcademicYearId == request.AcademicYearId.Value);

        if (request.ClassId.HasValue)
            admissionsQuery = admissionsQuery.Where(x => x.AcademicClassId == request.ClassId.Value);

        if (request.SectionId.HasValue)
            admissionsQuery = admissionsQuery.Where(x => x.SectionId == request.SectionId.Value);

        var dashboardQuery = admissionsQuery.Select(x => new FeeDashboardRowResponse
        {
            StudentId = x.StudentId,
            AdmissionId = x.AdmissionId,
            StudentName = x.StudentName,
            AdmissionNo = x.AdmissionNo,
            AcademicYear = x.AcademicYearName,
            ClassName = x.ClassName,
            SectionName = x.SectionName,
            RollNo = x.RollNo,

            FeeStructureName =
                (from assignment in _db.StudentFeeAssignments.AsNoTracking()
                 join structure in _db.FeeStructures.AsNoTracking()
                     on assignment.FeeStructureId equals structure.Id
                 where assignment.TenantId == tenantId
                       && assignment.BranchId == branchId
                       && assignment.StudentAdmissionId == x.AdmissionId
                       && assignment.IsActive
                 select structure.Name)
                .FirstOrDefault(),

            HasAssignment = _db.StudentFeeAssignments.AsNoTracking()
                .Any(a => a.TenantId == tenantId
                          && a.BranchId == branchId
                          && a.StudentAdmissionId == x.AdmissionId
                          && a.IsActive),

            ChargeCount = _db.StudentCharges.AsNoTracking()
                .Count(c => c.TenantId == tenantId
                            && c.BranchId == branchId
                            && c.StudentAdmissionId == x.AdmissionId
                            && !c.IsCancelled),

            PendingChargeCount = _db.StudentCharges.AsNoTracking()
                .Count(c => c.TenantId == tenantId
                            && c.BranchId == branchId
                            && c.StudentAdmissionId == x.AdmissionId
                            && !c.IsCancelled
                            && (c.Amount + c.FineAmount - c.DiscountAmount - c.PaidAmount) > 0),

            ReceiptCount = _db.FeeReceipts.AsNoTracking()
                .Count(r => r.TenantId == tenantId
                            && r.BranchId == branchId
                            && r.StudentAdmissionId == x.AdmissionId
                            && !r.IsCancelled),

            TotalAmount = _db.StudentCharges.AsNoTracking()
                .Where(c => c.TenantId == tenantId
                            && c.BranchId == branchId
                            && c.StudentAdmissionId == x.AdmissionId
                            && !c.IsCancelled)
                .Select(c => (decimal?)(c.Amount + c.FineAmount - c.DiscountAmount))
                .Sum() ?? 0m,

            TotalPaid = _db.StudentCharges.AsNoTracking()
                .Where(c => c.TenantId == tenantId
                            && c.BranchId == branchId
                            && c.StudentAdmissionId == x.AdmissionId
                            && !c.IsCancelled)
                .Select(c => (decimal?)c.PaidAmount)
                .Sum() ?? 0m,

            TotalBalance = _db.StudentCharges.AsNoTracking()
                .Where(c => c.TenantId == tenantId
                            && c.BranchId == branchId
                            && c.StudentAdmissionId == x.AdmissionId
                            && !c.IsCancelled)
                .Select(c => (decimal?)(c.Amount + c.FineAmount - c.DiscountAmount - c.PaidAmount))
                .Sum() ?? 0m,

            HasCharges = _db.StudentCharges.AsNoTracking()
                .Any(c => c.TenantId == tenantId
                          && c.BranchId == branchId
                          && c.StudentAdmissionId == x.AdmissionId
                          && !c.IsCancelled),

            CanCollect = _db.StudentCharges.AsNoTracking()
                .Any(c => c.TenantId == tenantId
                          && c.BranchId == branchId
                          && c.StudentAdmissionId == x.AdmissionId
                          && !c.IsCancelled
                          && (c.Amount + c.FineAmount - c.DiscountAmount - c.PaidAmount) > 0),

            LastReceiptDate = _db.FeeReceipts.AsNoTracking()
                .Where(r => r.TenantId == tenantId
                            && r.BranchId == branchId
                            && r.StudentAdmissionId == x.AdmissionId
                            && !r.IsCancelled)
                .OrderByDescending(r => r.ReceiptDate)
                .Select(r => (DateTime?)r.ReceiptDate)
                .FirstOrDefault(),

            WorkflowStatus =
                !_db.StudentFeeAssignments.AsNoTracking()
                    .Any(a => a.TenantId == tenantId
                              && a.BranchId == branchId
                              && a.StudentAdmissionId == x.AdmissionId
                              && a.IsActive)
                    ? "Plan Missing"
                    : !_db.StudentCharges.AsNoTracking()
                        .Any(c => c.TenantId == tenantId
                                  && c.BranchId == branchId
                                  && c.StudentAdmissionId == x.AdmissionId
                                  && !c.IsCancelled)
                        ? "Charges Pending"
                        : ((_db.StudentCharges.AsNoTracking()
                                .Where(c => c.TenantId == tenantId
                                            && c.BranchId == branchId
                                            && c.StudentAdmissionId == x.AdmissionId
                                            && !c.IsCancelled)
                                .Select(c => (decimal?)(c.Amount + c.FineAmount - c.DiscountAmount - c.PaidAmount))
                                .Sum() ?? 0m) <= 0m)
                            ? "Paid"
                            : ((_db.StudentCharges.AsNoTracking()
                                    .Where(c => c.TenantId == tenantId
                                                && c.BranchId == branchId
                                                && c.StudentAdmissionId == x.AdmissionId
                                                && !c.IsCancelled)
                                    .Select(c => (decimal?)c.PaidAmount)
                                    .Sum() ?? 0m) > 0m)
                                ? "Partial"
                                : "Collectible"
        });

        if (!string.IsNullOrWhiteSpace(request.WorkflowStatus))
            dashboardQuery = dashboardQuery.Where(x => x.WorkflowStatus == request.WorkflowStatus);

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            dashboardQuery = request.SortBy.Trim().ToLowerInvariant() switch
            {
                "studentname" => request.SortDescending
                    ? dashboardQuery.OrderByDescending(x => x.StudentName)
                    : dashboardQuery.OrderBy(x => x.StudentName),

                "admissionno" => request.SortDescending
                    ? dashboardQuery.OrderByDescending(x => x.AdmissionNo)
                    : dashboardQuery.OrderBy(x => x.AdmissionNo),

                "classname" => request.SortDescending
                    ? dashboardQuery.OrderByDescending(x => x.ClassName)
                    : dashboardQuery.OrderBy(x => x.ClassName),

                "totalamount" => request.SortDescending
                    ? dashboardQuery.OrderByDescending(x => x.TotalAmount)
                    : dashboardQuery.OrderBy(x => x.TotalAmount),

                "totalpaid" => request.SortDescending
                    ? dashboardQuery.OrderByDescending(x => x.TotalPaid)
                    : dashboardQuery.OrderBy(x => x.TotalPaid),

                "totalbalance" => request.SortDescending
                    ? dashboardQuery.OrderByDescending(x => x.TotalBalance)
                    : dashboardQuery.OrderBy(x => x.TotalBalance),

                "workflowstatus" => request.SortDescending
                    ? dashboardQuery.OrderByDescending(x => x.WorkflowStatus)
                    : dashboardQuery.OrderBy(x => x.WorkflowStatus),

                _ => request.SortDescending
                    ? dashboardQuery.OrderByDescending(x => x.StudentName)
                    : dashboardQuery.OrderBy(x => x.StudentName)
            };
        }
        else
        {
            dashboardQuery = dashboardQuery.OrderBy(x => x.StudentName);
        }

        var summary = new FeeDashboardSummaryResponse
        {
            TotalStudents = await dashboardQuery.CountAsync(cancellationToken),
            AssignedPlans = await dashboardQuery.CountAsync(x => x.HasAssignment, cancellationToken),
            CollectibleStudents = await dashboardQuery.CountAsync(
                x => x.WorkflowStatus == "Collectible" || x.WorkflowStatus == "Partial",
                cancellationToken),
            FullyPaidStudents = await dashboardQuery.CountAsync(x => x.WorkflowStatus == "Paid", cancellationToken),
            TotalAmount = await dashboardQuery.Select(x => x.TotalAmount).SumAsync(cancellationToken),
            TotalPaid = await dashboardQuery.Select(x => x.TotalPaid).SumAsync(cancellationToken),
            TotalBalance = await dashboardQuery.Select(x => x.TotalBalance).SumAsync(cancellationToken)
        };

        var paged = await dashboardQuery.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize);

        return new FeeDashboardResponse
        {
            Summary = summary,
            Students = paged
        };
    }
}