using Shala.Application.Common;
using Shala.Domain.Entities.Fees;

namespace Shala.Application.Repositories.Fees;

public interface IFeeStructureRepository : IGenericRepository<FeeStructure>
{
    Task<List<FeeStructure>> GetAllAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);
    Task<FeeStructure?> GetByIdAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default);
    Task<FeeStructure?> GetWithItemsAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default);
}