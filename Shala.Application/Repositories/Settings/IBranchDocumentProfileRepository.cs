using Shala.Domain.Entities.Settings;

namespace Shala.Application.Repositories.Settings
{
    public interface IBranchDocumentProfileRepository
    {
        Task<BranchDocumentProfile?> GetByScopeAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<BranchDocumentProfile?> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            BranchDocumentProfile entity,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}