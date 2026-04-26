using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Infrastructure.Data;
using Shala.Shared.Common;
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
    request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
    request.PageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100);

    var admissionsQuery =
        from a in _db.StudentAdmissions.AsNoTracking()
        join s in _db.Students.AsNoTracking() on a.StudentId equals s.Id
        join y in _db.AcademicYears.AsNoTracking() on a.AcademicYearId equals y.Id
        join c in _db.AcademicClasses.AsNoTracking() on a.AcademicClassId equals c.Id
        join sec in _db.Sections.AsNoTracking() on a.SectionId equals sec.Id into secJoin
        from sec in secJoin.DefaultIfEmpty()
        where a.TenantId == tenantId
              && a.BranchId == branchId
              && a.IsCurrent
        select new
        {
            StudentId = s.Id,
            AdmissionId = a.Id,
            StudentName = ((s.FirstName ?? "") + " " + (s.MiddleName ?? "") + " " + (s.LastName ?? "")).Trim(),
            AdmissionNo = a.AdmissionNo,
            AcademicYear = y.Name,
            ClassName = c.Name,
            SectionName = sec != null ? sec.Name : null,
            RollNo = a.RollNo,
            a.AcademicYearId,
            a.AcademicClassId,
            a.SectionId
        };

    if (!string.IsNullOrWhiteSpace(request.SearchText))
    {
        var search = request.SearchText.Trim();

        admissionsQuery = admissionsQuery.Where(x =>
            x.StudentName.Contains(search) ||
            x.AdmissionNo.Contains(search) ||
            (x.RollNo != null && x.RollNo.Contains(search)));
    }

    if (request.AcademicYearId.HasValue)
        admissionsQuery = admissionsQuery.Where(x => x.AcademicYearId == request.AcademicYearId.Value);

    if (request.ClassId.HasValue)
        admissionsQuery = admissionsQuery.Where(x => x.AcademicClassId == request.ClassId.Value);

    if (request.SectionId.HasValue)
        admissionsQuery = admissionsQuery.Where(x => x.SectionId == request.SectionId.Value);

    var admissions = await admissionsQuery.ToListAsync(cancellationToken);

    var admissionIds = admissions.Select(x => x.AdmissionId).ToList();

    var chargeMap = await _db.StudentCharges
        .AsNoTracking()
       .Where(x =>
    x.TenantId == tenantId &&
    x.BranchId == branchId &&
    !x.IsCancelled &&
    x.StudentAdmissionId.HasValue &&
    admissionIds.Contains(x.StudentAdmissionId.Value))
.GroupBy(x => x.StudentAdmissionId!.Value)
        .Select(g => new
        {
            AdmissionId = g.Key,
            ChargeCount = g.Count(),
            PendingChargeCount = g.Count(x => (x.Amount + x.FineAmount - x.DiscountAmount - x.PaidAmount) > 0),
            TotalAmount = g.Sum(x => x.Amount + x.FineAmount - x.DiscountAmount),
            TotalPaid = g.Sum(x => x.PaidAmount),
            TotalBalance = g.Sum(x => x.Amount + x.FineAmount - x.DiscountAmount - x.PaidAmount)
        })
        .ToDictionaryAsync(x => x.AdmissionId, cancellationToken);

    var receiptMap = await _db.FeeReceipts
        .AsNoTracking()
       .Where(x =>
    x.TenantId == tenantId &&
    x.BranchId == branchId &&
    !x.IsCancelled &&
    x.StudentAdmissionId.HasValue &&
    admissionIds.Contains(x.StudentAdmissionId.Value))
.GroupBy(x => x.StudentAdmissionId!.Value)
        .Select(g => new
        {
            AdmissionId = g.Key,
            ReceiptCount = g.Count(),
            LastReceiptDate = g.Max(x => (DateTime?)x.ReceiptDate)
        })
        .ToDictionaryAsync(x => x.AdmissionId, cancellationToken);

    var assignmentMap = await _db.StudentFeeAssignments
        .AsNoTracking()
        .Where(x =>
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            x.IsActive &&
            admissionIds.Contains(x.StudentAdmissionId))
        .Select(x => new
        {
            x.StudentAdmissionId,
            FeeStructureName = x.FeeStructure.Name
        })
        .GroupBy(x => x.StudentAdmissionId)
        .Select(g => new
        {
            AdmissionId = g.Key,
            FeeStructureName = g.Select(x => x.FeeStructureName).FirstOrDefault()
        })
        .ToDictionaryAsync(x => x.AdmissionId, cancellationToken);

    var rows = admissions.Select(x =>
    {
        chargeMap.TryGetValue(x.AdmissionId, out var charge);
        receiptMap.TryGetValue(x.AdmissionId, out var receipt);
        assignmentMap.TryGetValue(x.AdmissionId, out var assignment);

        var hasAssignment = assignment != null;
        var hasCharges = charge != null && charge.ChargeCount > 0;
        var totalBalance = charge?.TotalBalance ?? 0m;
        var totalPaid = charge?.TotalPaid ?? 0m;

        var workflowStatus =
            !hasAssignment ? "Plan Missing" :
            !hasCharges ? "Charges Pending" :
            totalBalance <= 0 ? "Paid" :
            totalPaid > 0 ? "Partial" :
            "Collectible";

        return new FeeDashboardRowResponse
        {
            StudentId = x.StudentId,
            AdmissionId = x.AdmissionId,
            StudentName = x.StudentName,
            AdmissionNo = x.AdmissionNo,
            AcademicYear = x.AcademicYear,
            ClassName = x.ClassName,
            SectionName = x.SectionName,
            RollNo = x.RollNo,
            FeeStructureName = assignment?.FeeStructureName,

            ChargeCount = charge?.ChargeCount ?? 0,
            PendingChargeCount = charge?.PendingChargeCount ?? 0,
            ReceiptCount = receipt?.ReceiptCount ?? 0,

            TotalAmount = charge?.TotalAmount ?? 0m,
            TotalPaid = totalPaid,
            TotalBalance = totalBalance,

            LastReceiptDate = receipt?.LastReceiptDate,

            HasAssignment = hasAssignment,
            HasCharges = hasCharges,
            CanCollect = totalBalance > 0,
            WorkflowStatus = workflowStatus
        };
    }).ToList();

    if (!string.IsNullOrWhiteSpace(request.WorkflowStatus))
        rows = rows.Where(x => x.WorkflowStatus == request.WorkflowStatus).ToList();

    rows = rows.OrderBy(x => x.StudentName).ToList();

    var totalCount = rows.Count;

    var summary = new FeeDashboardSummaryResponse
    {
        TotalStudents = totalCount,
        AssignedPlans = rows.Count(x => x.HasAssignment),
        CollectibleStudents = rows.Count(x => x.WorkflowStatus is "Collectible" or "Partial"),
        FullyPaidStudents = rows.Count(x => x.WorkflowStatus == "Paid"),
        TotalAmount = rows.Sum(x => x.TotalAmount),
        TotalPaid = rows.Sum(x => x.TotalPaid),
        TotalBalance = rows.Sum(x => x.TotalBalance)
    };

    var pagedRows = rows
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToList();

    return new FeeDashboardResponse
    {
        Summary = summary,
        Students = new PagedResult<FeeDashboardRowResponse>
        {
            Items = pagedRows,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        }
    };
}

    private static IQueryable<FeeDashboardRowResponse> ApplySorting(
        IQueryable<FeeDashboardRowResponse> query,
        FeeDashboardRequest request)
    {
        var sortBy = request.SortBy?.Trim().ToLowerInvariant();

        return sortBy switch
        {
            "studentname" => request.SortDescending
                ? query.OrderByDescending(x => x.StudentName)
                : query.OrderBy(x => x.StudentName),

            "admissionno" => request.SortDescending
                ? query.OrderByDescending(x => x.AdmissionNo)
                : query.OrderBy(x => x.AdmissionNo),

            "classname" => request.SortDescending
                ? query.OrderByDescending(x => x.ClassName)
                : query.OrderBy(x => x.ClassName),

            "totalamount" => request.SortDescending
                ? query.OrderByDescending(x => x.TotalAmount)
                : query.OrderBy(x => x.TotalAmount),

            "totalpaid" => request.SortDescending
                ? query.OrderByDescending(x => x.TotalPaid)
                : query.OrderBy(x => x.TotalPaid),

            "totalbalance" => request.SortDescending
                ? query.OrderByDescending(x => x.TotalBalance)
                : query.OrderBy(x => x.TotalBalance),

            "workflowstatus" => request.SortDescending
                ? query.OrderByDescending(x => x.WorkflowStatus)
                : query.OrderBy(x => x.WorkflowStatus),

            _ => request.SortDescending
                ? query.OrderByDescending(x => x.StudentName)
                : query.OrderBy(x => x.StudentName)
        };
    }
}