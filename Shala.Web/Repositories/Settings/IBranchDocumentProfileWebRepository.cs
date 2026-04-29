using Shala.Shared.Requests.Settings;
using Shala.Shared.Responses.Settings;

namespace Shala.Web.Repositories.Settings
{
    public interface IBranchDocumentProfileWebRepository
    {
        Task<BranchDocumentProfileResponse?> GetAsync(
            CancellationToken cancellationToken = default);

        Task<BranchDocumentProfileResponse> SaveAsync(
            SaveBranchDocumentProfileRequest request,
            CancellationToken cancellationToken = default);
    }
}