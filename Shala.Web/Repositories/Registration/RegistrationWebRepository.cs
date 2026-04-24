using Shala.Shared.Common;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.Registration
{
    public sealed class RegistrationWebRepository : IRegistrationWebRepository
    {
        private readonly IHttpService _httpService;

        public RegistrationWebRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<int> CreateAsync(
            CreateRegistrationRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PostAsync<CreateRegistrationRequest, int>(
                "api/registrations",
                request);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<PagedResult<RegistrationDto>> GetPagedAsync(
            PagedRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PostAsync<PagedRequest, PagedResult<RegistrationDto>>(
                "api/registrations/paged",
                request);

            EnsureSuccess(response);
            return response.ServerResponse ?? new PagedResult<RegistrationDto>();
        }

        public async Task<List<RegistrationDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.GetAsync<List<RegistrationDto>>(
                "api/registrations");

            EnsureSuccess(response);
            return response.ServerResponse ?? new List<RegistrationDto>();
        }

        public async Task<RegistrationDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.GetAsync<RegistrationDto>(
                $"api/registrations/{id}");

            EnsureSuccess(response);
            return response.ServerResponse;
        }

        public async Task UpdateAsync(
            int id,
            UpdateRegistrationRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PutAsync<UpdateRegistrationRequest, object?>(
                $"api/registrations/{id}",
                request);

            EnsureSuccess(response);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.DeleteAsync(
                $"api/registrations/{id}");

            EnsureSuccess(response);
        }

       

        public async Task<RegistrationFeeResponse> SaveWithFeeAsync(
            SaveRegistrationWithFeeRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PostAsync<SaveRegistrationWithFeeRequest, RegistrationFeeResponse>(
                "api/registrations/save-with-fee",
                request);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<RegistrationFeeResponse> CollectFeeAsync(
            int id,
            CollectRegistrationFeeRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.PostAsync<CollectRegistrationFeeRequest, RegistrationFeeResponse>(
                $"api/registrations/{id}/collect-fee",
                request);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<RegistrationReceiptResponse> GetReceiptAsync(
            int receiptId,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpService.GetAsync<RegistrationReceiptResponse>(
                $"api/registrations/receipt/{receiptId}");

            EnsureSuccess(response);
            return response.ServerResponse!;
        }




        public async Task<ConvertRegistrationResponse> ConvertAsync(
    int id,
    ConvertRegistrationRequest request,
    CancellationToken cancellationToken = default)
        {
            request ??= new ConvertRegistrationRequest();

            var response = await _httpService.PostAsync<ConvertRegistrationRequest, ConvertRegistrationResponse>(
                $"api/registrations/{id}/convert",
                request);

            EnsureSuccess(response);
            return response.ServerResponse ?? new ConvertRegistrationResponse();
        }

        public async Task CancelReceiptAsync(
    int receiptId,
    CancelRegistrationReceiptRequest request,
    CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var response = await _httpService.PostAsync<
                CancelRegistrationReceiptRequest,
                object?>(
                $"api/registrations/receipt/{receiptId}/cancel",
                request);

            EnsureSuccess(response);
        }

        public async Task RefundReceiptAsync(
            int receiptId,
            RefundRegistrationReceiptRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var response = await _httpService.PostAsync<
                RefundRegistrationReceiptRequest,
                object?>(
                $"api/registrations/receipt/{receiptId}/refund",
                request);

            EnsureSuccess(response);
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