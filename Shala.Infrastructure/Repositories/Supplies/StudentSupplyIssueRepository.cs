using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Supplies;
using Shala.Domain.Entities.Supplies;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Supplies;

public class StudentSupplyIssueRepository : GenericRepository<StudentSupplyIssue>, IStudentSupplyIssueRepository
{
    public StudentSupplyIssueRepository(AppDbContext db) : base(db)
    {
    }

    private IQueryable<StudentSupplyIssue> DetailQuery()
    {
        return _table
            .Include(x => x.Student)
            .Include(x => x.StudentAdmission)
                .ThenInclude(x => x.AcademicClass)
            .Include(x => x.StudentAdmission)
                .ThenInclude(x => x.Section)
            .Include(x => x.Items)
            .Include(x => x.Payments);
    }

    public Task<StudentSupplyIssue?> GetDetailAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return DetailQuery().FirstOrDefaultAsync(x =>
            x.Id == id &&
            x.TenantId == tenantId &&
            x.BranchId == branchId,
            cancellationToken);
    }

    public Task<List<StudentSupplyIssue>> GetRecentAsync(int tenantId, int branchId, int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var query = DetailQuery()
            .Where(x => x.TenantId == tenantId &&
                        x.BranchId == branchId &&
                        !x.IsCancelled);

        if (academicYearId.HasValue)
            query = query.Where(x => x.AcademicYearId == academicYearId.Value);

        return query.OrderByDescending(x => x.IssueDate)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public Task<List<StudentSupplyIssue>> GetStudentHistoryAsync(int tenantId, int branchId, int studentId, int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var query = DetailQuery()
            .Where(x => x.TenantId == tenantId &&
                        x.BranchId == branchId &&
                        x.StudentId == studentId &&
                        !x.IsCancelled);

        if (academicYearId.HasValue)
            query = query.Where(x => x.AcademicYearId == academicYearId.Value);

        return query.OrderByDescending(x => x.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<StudentSupplyIssue>> GetPendingDuesAsync(int tenantId, int branchId, int? academicYearId = null, int? studentId = null, CancellationToken cancellationToken = default)
    {
        var query = DetailQuery()
            .Where(x => x.TenantId == tenantId &&
                        x.BranchId == branchId &&
                        !x.IsCancelled &&
                        x.DueAmount > 0);

        if (academicYearId.HasValue)
            query = query.Where(x => x.AcademicYearId == academicYearId.Value);

        if (studentId.HasValue)
            query = query.Where(x => x.StudentId == studentId.Value);

        return query.OrderByDescending(x => x.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetTodayIssueCountAsync(int tenantId, int branchId, DateTime date, CancellationToken cancellationToken = default)
    {
        var from = date.Date;
        var to = from.AddDays(1);

        return _table.CountAsync(x =>
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            !x.IsCancelled &&
            x.IssueDate >= from &&
            x.IssueDate < to,
            cancellationToken);
    }

    public async Task<decimal> GetPendingDueTotalAsync(int tenantId, int branchId, int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var query = _table.Where(x =>
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            !x.IsCancelled &&
            x.DueAmount > 0);

        if (academicYearId.HasValue)
            query = query.Where(x => x.AcademicYearId == academicYearId.Value);

        return await query.SumAsync(x => (decimal?)x.DueAmount, cancellationToken) ?? 0m;
    }

    public async Task<string> GenerateNextIssueNoAsync(int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"SUP-{year}-";

        var count = await _table.CountAsync(x =>
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            x.IssueNo.StartsWith(prefix),
            cancellationToken);

        return $"{prefix}{count + 1:0000}";
    }
}