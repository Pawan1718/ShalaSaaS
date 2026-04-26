using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;


namespace Shala.Web.Repositories.PlatformRepo;

public interface IBranchRepository
{
    Task<List<BranchResponse>> GetAllAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<BranchResponse?> GetByIdAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);
    Task<BranchResponse?> CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken = default);
    Task<BranchResponse?> UpdateAsync(int tenantId, int branchId, UpdateBranchRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);
}