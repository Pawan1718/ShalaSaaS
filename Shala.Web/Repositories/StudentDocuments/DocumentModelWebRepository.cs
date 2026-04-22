using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.StudentDocuments
{
    public sealed class DocumentModelWebRepository : IDocumentModelWebRepository
    {
        private readonly IHttpService _httpService;
        private const string BaseRoute = "api/student-document-models";
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

        public DocumentModelWebRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<DocumentModelResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.GetAsync<List<DocumentModelResponse>>(BaseRoute, cts.Token);
            EnsureSuccess(response);
            return response.ServerResponse ?? new List<DocumentModelResponse>();
        }

        public async Task<List<DocumentModelResponse>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.GetAsync<List<DocumentModelResponse>>($"{BaseRoute}/active", cts.Token);
            EnsureSuccess(response);
            return response.ServerResponse ?? new List<DocumentModelResponse>();
        }

        public async Task<DocumentModelResponse> CreateAsync(CreateDocumentModelRequest request, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.PostAsync<CreateDocumentModelRequest, DocumentModelResponse>(BaseRoute, request, cts.Token);
            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<DocumentModelResponse> UpdateAsync(UpdateDocumentModelRequest request, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.PutAsync<UpdateDocumentModelRequest, DocumentModelResponse>(BaseRoute, request, cts.Token);
            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<DocumentModelResponse> ToggleStatusAsync(ToggleDocumentModelStatusRequest request, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.PostAsync<ToggleDocumentModelStatusRequest, DocumentModelResponse>(
                $"{BaseRoute}/toggle-status",
                request,
                cts.Token);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        private static void EnsureSuccess<T>(ServerResponseHelper<T>? response)
        {
            if (response is null)
                throw new Exception("No response received from server.");

            if (response.IsSuccess)
                return;

            var statusCode = response.ResponseMessage is not null
                ? ((int)response.ResponseMessage.StatusCode).ToString()
                : "unknown";

            var message = string.IsNullOrWhiteSpace(response.Message)
                ? $"Request failed with status code {statusCode}."
                : response.Message;

            throw new Exception(message);
        }
    }
}