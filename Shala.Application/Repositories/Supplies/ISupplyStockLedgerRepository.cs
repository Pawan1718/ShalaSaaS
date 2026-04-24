using Shala.Application.Common;
using Shala.Domain.Entities.Supplies;

namespace Shala.Application.Repositories.Supplies;

public interface ISupplyStockLedgerRepository : IGenericRepository<SupplyStockLedger>
{
    Task<List<SupplyStockLedger>> GetByItemIdAsync(int supplyItemId, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<List<SupplyStockLedger>> GetHistoryAsync(int tenantId, int branchId, DateTime? fromDate = null, DateTime? toDate = null, int? supplyItemId = null, CancellationToken cancellationToken = default);
}