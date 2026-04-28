using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Supplies;
using Shala.Domain.Entities.Supplies;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Supplies;

public class SupplyStockLedgerRepository : GenericRepository<SupplyStockLedger>, ISupplyStockLedgerRepository
{
    public SupplyStockLedgerRepository(AppDbContext db) : base(db)
    {
    }

    public Task<List<SupplyStockLedger>> GetByItemIdAsync(int supplyItemId, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return _table
            .Include(x => x.SupplyItem)
            .Where(x => x.SupplyItemId == supplyItemId &&
                        x.TenantId == tenantId &&
                        x.BranchId == branchId)
            .OrderByDescending(x => x.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SupplyStockLedger>> GetHistoryAsync(int tenantId, int branchId, DateTime? fromDate = null, DateTime? toDate = null, int? supplyItemId = null, CancellationToken cancellationToken = default)
    {
        var query = _table
            .Include(x => x.SupplyItem)
            .Where(x => x.TenantId == tenantId &&
                        x.BranchId == branchId);

        if (supplyItemId.HasValue)
            query = query.Where(x => x.SupplyItemId == supplyItemId.Value);

        if (fromDate.HasValue)
            query = query.Where(x => x.MovementDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(x => x.MovementDate < toDate.Value.Date.AddDays(1));

        return query.OrderByDescending(x => x.MovementDate)
            .ToListAsync(cancellationToken);
    }
}