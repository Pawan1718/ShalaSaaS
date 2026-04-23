using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.Registration.RegistrationConfig
{
    public sealed class RegistrationProspectusConfigurationWebRepository : IRegistrationProspectusConfigurationWebRepository
    {
        private readonly IHttpService _httpService;

        public RegistrationProspectusConfigurationWebRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<RegistrationProspectusConfigurationResponse?> GetAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.GetAsync<RegistrationProspectusConfigurationResponse>(
                "api/registration/prospectus-configuration");

            EnsureSuccess(response);
            return response.ServerResponse;
        }

        public async Task<RegistrationProspectusConfigurationResponse> SaveAsync(
            SaveRegistrationProspectusConfigurationRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PostAsync<SaveRegistrationProspectusConfigurationRequest, RegistrationProspectusConfigurationResponse>(
                "api/registration/prospectus-configuration",
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