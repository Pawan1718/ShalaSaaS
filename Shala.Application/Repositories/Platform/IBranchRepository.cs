using Shala.Application.Common;
using Shala.Domain.Entities.Platform;

namespace Shala.Application.Repositories.Platform;

public interface IBranchRepository : IGenericRepository<Branch>
{
    Task<Branch?> GetByIdAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<List<Branch>> GetAllAsync(int tenantId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(int tenantId, string code, int? excludeBranchId = null, CancellationToken cancellationToken = default);
}