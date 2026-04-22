using Shala.Shared.Responses.Registration;
using Shala.Web.Repositories.Registration;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.Registration
{
    public sealed class RegistrationFeeHeadLookupWebRepository : IRegistrationFeeHeadLookupWebRepository
    {
        private readonly IHttpService _httpService;

        public RegistrationFeeHeadLookupWebRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<RegistrationFeeHeadLookupResponse>> GetAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.GetAsync<List<RegistrationFeeHeadLookupResponse>>(
                "api/registration/fee-heads");

            EnsureSuccess(response);
            return response.ServerResponse ?? new List<RegistrationFeeHeadLookupResponse>();
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