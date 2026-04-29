using Shala.Application.Common;
using Shala.Domain.Entities.Supplies;

namespace Shala.Application.Repositories.Supplies;

public interface ISupplyItemRepository : IGenericRepository<SupplyItem>
{
    Task<List<SupplyItem>> GetAllAsync(int tenantId, int branchId, bool activeOnly = false, CancellationToken cancellationToken = default);

    Task<SupplyItem?> GetByIdAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<SupplyItem?> GetByCodeAsync(string code, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<List<SupplyItem>> GetByIdsAsync(List<int> ids, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<bool> IsInUseAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<List<SupplyItem>> GetLowStockAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);
}