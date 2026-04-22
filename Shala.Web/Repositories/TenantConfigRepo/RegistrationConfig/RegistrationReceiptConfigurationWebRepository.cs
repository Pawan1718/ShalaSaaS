using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.TenantConfig
{
    public sealed class RegistrationReceiptConfigurationWebRepository : IRegistrationReceiptConfigurationWebRepository
    {
        private readonly IHttpService _httpService;

        public RegistrationReceiptConfigurationWebRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<RegistrationReceiptConfigurationResponse?> GetAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.GetAsync<RegistrationReceiptConfigurationResponse>(
                "api/registration/receipt-configuration");

            EnsureSuccess(response);
            return response.ServerResponse;
        }

        public async Task<RegistrationReceiptConfigurationResponse> SaveAsync(
            SaveRegistrationReceiptConfigurationRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PostAsync<SaveRegistrationReceiptConfigurationRequest, RegistrationReceiptConfigurationResponse>(
                "api/registration/receipt-configuration",
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