using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.TenantConfig
{
    public sealed class RegistrationFeeConfigurationWebRepository : IRegistrationFeeConfigurationWebRepository
    {
        private readonly IHttpService _httpService;

        public RegistrationFeeConfigurationWebRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<RegistrationFeeConfigurationResponse?> GetAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.GetAsync<RegistrationFeeConfigurationResponse>(
                "api/registration/fee-configuration");

            EnsureSuccess(response);
            return response.ServerResponse;
        }

        public async Task<RegistrationFeeConfigurationResponse> SaveAsync(
            SaveRegistrationFeeConfigurationRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PostAsync<SaveRegistrationFeeConfigurationRequest, RegistrationFeeConfigurationResponse>(
                "api/registration/fee-configuration",
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