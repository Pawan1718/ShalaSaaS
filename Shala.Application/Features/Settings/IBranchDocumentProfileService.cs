using Shala.Shared.Requests.Settings;
using Shala.Shared.Responses.Settings;

namespace Shala.Application.Features.Settings
{
    public interface IBranchDocumentProfileService
    {
        Task<BranchDocumentProfileResponse?> GetAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<BranchDocumentProfileResponse> SaveAsync(
            int tenantId,
            int branchId,
            SaveBranchDocumentProfileRequest request,
            CancellationToken cancellationToken = default);
    }
}