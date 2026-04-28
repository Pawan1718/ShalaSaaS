using Shala.Shared.Requests.Settings;
using Shala.Shared.Responses.Settings;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.Settings
{
    public sealed class BranchDocumentProfileWebRepository : IBranchDocumentProfileWebRepository
    {
        private readonly IHttpService _httpService;

        public BranchDocumentProfileWebRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<BranchDocumentProfileResponse?> GetAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.GetAsync<BranchDocumentProfileResponse>(
                "api/settings/branch-document-profile");

            EnsureSuccess(response);
            return response.ServerResponse;
        }

        public async Task<BranchDocumentProfileResponse> SaveAsync(
            SaveBranchDocumentProfileRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PostAsync<SaveBranchDocumentProfileRequest, BranchDocumentProfileResponse>(
                "api/settings/branch-document-profile",
                request);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        private static void EnsureSuccess<T>(ServerResponseHelper<T> response)
        {
            if (response.IsSuccess)
                return;

            var message = string.IsNullOrWhiteSpace(response.Message)
                ? $"Request failed with status code {(int)response.ResponseMessage.StatusCode}."
                : response.Message;

            throw new Exception(message);
        }
    }
}