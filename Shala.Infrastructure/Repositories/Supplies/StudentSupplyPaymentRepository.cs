using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Supplies;
using Shala.Domain.Entities.Supplies;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Supplies;

public class StudentSupplyPaymentRepository : GenericRepository<StudentSupplyPayment>, IStudentSupplyPaymentRepository
{
    public StudentSupplyPaymentRepository(AppDbContext db) : base(db)
    {
    }

    public Task<List<StudentSupplyPayment>> GetByIssueIdAsync(int issueId, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return _table
            .Where(x => x.StudentSupplyIssueId == issueId &&
                        x.TenantId == tenantId &&
                        x.BranchId == branchId)
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetCollectionTotalAsync(int tenantId, int branchId, DateTime fromDate, DateTime toDate, int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var query = _table.Where(x =>
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            x.PaymentDate >= fromDate &&
            x.PaymentDate < toDate);

        if (academicYearId.HasValue)
            query = query.Where(x => x.AcademicYearId == academicYearId.Value);

        return await query.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
    }
}