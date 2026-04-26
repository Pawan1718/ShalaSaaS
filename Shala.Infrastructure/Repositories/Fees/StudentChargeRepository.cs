using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Infrastructure.Data;
using Shala.Shared.Responses.Fees;

namespace Shala.Infrastructure.Repositories.Fees;

public class StudentChargeRepository : GenericRepository<StudentCharge>, IStudentChargeRepository
{
    public StudentChargeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<StudentCharge>> GetByStudentIdAsync(
        int studentId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.FeeHead)
            .Include(x => x.StudentAdmission)
            .Include(x => x.Allocations)
            .Where(x =>
                x.StudentId == studentId &&
                x.TenantId == tenantId &&
                x.BranchId == branchId)
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<StudentCharge>> GetByAssignmentIdAsync(
        int studentFeeAssignmentId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.FeeHead)
            .Include(x => x.Allocations)
            .Where(x =>
                x.StudentFeeAssignmentId == studentFeeAssignmentId &&
                x.TenantId == tenantId &&
                x.BranchId == branchId)
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentCharge?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.FeeHead)
            .Include(x => x.Student)
            .Include(x => x.StudentAdmission)
            .Include(x => x.StudentFeeAssignment)
            .Include(x => x.Allocations)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task AddRangeAsync(
        List<StudentCharge> entities,
        CancellationToken cancellationToken = default)
    {
        await _table.AddRangeAsync(entities, cancellationToken);
    }

    public Task DeleteRangeAsync(
        IEnumerable<StudentCharge> charges,
        CancellationToken cancellationToken = default)
    {
        var chargeList = charges.ToList();

        if (chargeList.Any(x => x.PaidAmount > 0))
            throw new InvalidOperationException("Paid charges cannot be deleted.");

        _table.RemoveRange(chargeList);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsHistoricalChargeForFeeHeadAsync(
        int studentId,
        int feeHeadId,
        int tenantId,
        int branchId,
        int? excludeAssignmentId = null,
        CancellationToken cancellationToken = default)
    {
        return _table.AnyAsync(x =>
            x.StudentId == studentId &&
            x.FeeHeadId == feeHeadId &&
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            !x.IsCancelled &&
            (!excludeAssignmentId.HasValue || x.StudentFeeAssignmentId != excludeAssignmentId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsAnyHistoricalChargeForFeeHeadAsync(
        int studentId,
        int feeHeadId,
        int tenantId,
        int branchId,
        int? excludeAssignmentId = null,
        CancellationToken cancellationToken = default)
    {
        return _table.AnyAsync(x =>
            x.StudentId == studentId &&
            x.FeeHeadId == feeHeadId &&
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            (!excludeAssignmentId.HasValue || x.StudentFeeAssignmentId != excludeAssignmentId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsChargeForFeeHeadInAcademicYearAsync(
        int studentId,
        int feeHeadId,
        int academicYear,
        int tenantId,
        int branchId,
        int? excludeAssignmentId = null,
        CancellationToken cancellationToken = default)
    {
        var fromDate = new DateTime(academicYear, 4, 1);
        var toDateExclusive = new DateTime(academicYear + 1, 4, 1);

        return _table.AnyAsync(x =>
            x.StudentId == studentId &&
            x.FeeHeadId == feeHeadId &&
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            !x.IsCancelled &&
            x.DueDate >= fromDate &&
            x.DueDate < toDateExclusive &&
            (!excludeAssignmentId.HasValue || x.StudentFeeAssignmentId != excludeAssignmentId.Value),
            cancellationToken);
    }
    public async Task<(IReadOnlyList<StudentChargeResponse> Items, int TotalCount)> GetPagedByStudentIdAsync(
    int studentId,
    int tenantId,
    int branchId,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var query = _table
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                x.TenantId == tenantId &&
                x.BranchId == branchId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new StudentChargeResponse
            {
                Id = x.Id,
                StudentId = x.StudentId ?? 0,
                StudentAdmissionId = x.StudentAdmissionId ?? 0,
                StudentFeeAssignmentId = x.StudentFeeAssignmentId ?? 0,
                FeeHeadId = x.FeeHeadId,
                ChargeLabel = x.ChargeLabel,
                PeriodLabel = x.PeriodLabel,
                DueDate = x.DueDate,
                Amount = x.Amount,
                DiscountAmount = x.DiscountAmount,
                FineAmount = x.FineAmount,
                PaidAmount = x.PaidAmount,
                NetAmount = x.NetAmount,
                BalanceAmount = x.BalanceAmount,
                IsSettled = x.IsSettled,
                IsCancelled = x.IsCancelled
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

}