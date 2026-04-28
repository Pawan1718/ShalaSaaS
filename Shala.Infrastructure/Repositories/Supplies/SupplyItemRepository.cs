using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Supplies;
using Shala.Domain.Entities.Supplies;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Supplies;

public class SupplyItemRepository : GenericRepository<SupplyItem>, ISupplyItemRepository
{
    private readonly AppDbContext _db;

    public SupplyItemRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public Task<List<SupplyItem>> GetAllAsync(int tenantId, int branchId, bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _table.Where(x => x.TenantId == tenantId && x.BranchId == branchId);

        if (activeOnly)
            query = query.Where(x => x.IsActive);

        return query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<SupplyItem?> GetByIdAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return _table.FirstOrDefaultAsync(x =>
            x.Id == id &&
            x.TenantId == tenantId &&
            x.BranchId == branchId,
            cancellationToken);
    }

    public Task<SupplyItem?> GetByCodeAsync(string code, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return _table.FirstOrDefaultAsync(x =>
            x.Code == code &&
            x.TenantId == tenantId &&
            x.BranchId == branchId,
            cancellationToken);
    }

    public Task<List<SupplyItem>> GetByIdsAsync(List<int> ids, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return _table
            .Where(x => ids.Contains(x.Id) &&
                        x.TenantId == tenantId &&
                        x.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsInUseAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return _db.Set<StudentSupplyIssueItem>()
            .AnyAsync(x =>
                x.SupplyItemId == id &&
                x.StudentSupplyIssue.TenantId == tenantId &&
                x.StudentSupplyIssue.BranchId == branchId,
                cancellationToken);
    }

    public Task<List<SupplyItem>> GetLowStockAsync(int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return _table
            .Where(x => x.TenantId == tenantId &&
                        x.BranchId == branchId &&
                        x.IsActive &&
                        x.CurrentStock <= x.MinimumStock)
            .OrderBy(x => x.CurrentStock)
            .ToListAsync(cancellationToken);
    }
}