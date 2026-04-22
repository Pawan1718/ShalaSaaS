using System.Net.Http.Headers;
using System.Text.Json;
using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;
using Shala.Web.Services.Http;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.StudentDocuments
{
    public sealed class StudentDocumentWebRepository : IStudentDocumentWebRepository
    {
        private readonly IHttpService _httpService;
        private readonly HttpClient _httpClient;
        private readonly ApiSession _session;
        private const string BaseRoute = "api/student-documents";
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

        public StudentDocumentWebRepository(
            IHttpService httpService,
            HttpClient httpClient,
            ApiSession session)
        {
            _httpService = httpService;
            _httpClient = httpClient;
            _session = session;
        }

        public async Task<List<StudentDocumentResponse>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.GetAsync<List<StudentDocumentResponse>>(
                $"{BaseRoute}/student/{studentId}",
                cts.Token);

            EnsureSuccess(response);
            return response.ServerResponse ?? new List<StudentDocumentResponse>();
        }

        public async Task<StudentDocumentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.GetAsync<StudentDocumentResponse?>(
                $"{BaseRoute}/{id}",
                cts.Token);

            EnsureSuccess(response);
            return response.ServerResponse;
        }

        public async Task<List<DocumentModelResponse>> GetActiveModelsAsync(CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.GetAsync<List<DocumentModelResponse>>(
                $"{BaseRoute}/active-models",
                cts.Token);

            EnsureSuccess(response);
            return response.ServerResponse ?? new List<DocumentModelResponse>();
        }

        public async Task<StudentDocumentResponse> UpdateAsync(UpdateStudentDocumentRequest request, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.PutAsync<UpdateStudentDocumentRequest, StudentDocumentResponse>(
                BaseRoute,
                request,
                cts.Token);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<StudentDocumentAnalysisResponse> AnalyzeAsync(AnalyzeStudentDocumentRequest request, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.PostAsync<AnalyzeStudentDocumentRequest, StudentDocumentAnalysisResponse>(
                $"{BaseRoute}/analyze",
                request,
                cts.Token);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<StudentDocumentResponse> VerifyAsync(VerifyStudentDocumentRequest request, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.PostAsync<VerifyStudentDocumentRequest, StudentDocumentResponse>(
                $"{BaseRoute}/verify",
                request,
                cts.Token);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<StudentDocumentResponse> ToggleStatusAsync(ToggleStudentDocumentStatusRequest request, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await _httpService.PostAsync<ToggleStudentDocumentStatusRequest, StudentDocumentResponse>(
                $"{BaseRoute}/toggle-status",
                request,
                cts.Token);

            EnsureSuccess(response);
            return response.ServerResponse!;
        }

        public async Task<StudentDocumentResponse> UploadAsync(UploadStudentDocumentWebRequest request, CancellationToken cancellationToken = default)
        {
            if (request.File is null)
                throw new InvalidOperationException("File is required.");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            await _session.InitializeAsync();

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(_session.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _session.Token);
            }

            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(request.StudentId.ToString()), nameof(request.StudentId));

            if (request.StudentRegistrationId.HasValue)
                form.Add(new StringContent(request.StudentRegistrationId.Value.ToString()), nameof(request.StudentRegistrationId));

            if (request.StudentAdmissionId.HasValue)
                form.Add(new StringContent(request.StudentAdmissionId.Value.ToString()), nameof(request.StudentAdmissionId));

            if (request.DocumentModelId.HasValue)
                form.Add(new StringContent(request.DocumentModelId.Value.ToString()), nameof(request.DocumentModelId));

            form.Add(new StringContent(request.DocumentType ?? string.Empty), nameof(request.DocumentType));
            form.Add(new StringContent(request.Title ?? string.Empty), nameof(request.Title));
            form.Add(new StringContent(request.IsRequired.ToString()), nameof(request.IsRequired));

            await using var stream = request.File.OpenReadStream(10 * 1024 * 1024, cts.Token);
            using var fileContent = new StreamContent(stream);

            if (!string.IsNullOrWhiteSpace(request.File.ContentType))
            {
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.File.ContentType);
            }

            form.Add(fileContent, nameof(request.File), request.File.Name);

            using var response = await _httpClient.PostAsync($"{BaseRoute}/upload", form, cts.Token);
            var raw = await response.Content.ReadAsStringAsync(cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.IsNullOrWhiteSpace(raw)
                    ? $"Upload failed with status code {(int)response.StatusCode}."
                    : raw);
            }

            var wrapper = JsonSerializer.Deserialize<ApiResponse<StudentDocumentResponse>>(
                raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (wrapper is null)
                throw new Exception("Invalid server response received.");

            if (!wrapper.Success || wrapper.Data is null)
                throw new Exception(wrapper.Message ?? "Failed to upload student document.");

            return wrapper.Data;
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