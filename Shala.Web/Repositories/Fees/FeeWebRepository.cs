//using Shala.Shared.Requests.Fees;
//using Shala.Shared.Responses.Fees;
//using Shala.Web.Services.Http;

//namespace Shala.Web.Repositories.Fees;

//public sealed class FeeWebRepository : IFeeWebRepository
//{
//    private readonly IHttpService _httpService;

//    public FeeWebRepository(IHttpService httpService)
//    {
//        _httpService = httpService;
//    }

//    public async Task<List<FeeHeadResponse>> GetFeeHeadsAsync(
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.GetAsync<List<FeeHeadResponse>>(
//            "api/fees/heads",
//            cancellationToken);

//        EnsureSuccess(response);
//        return response.ServerResponse ?? new List<FeeHeadResponse>();
//    }

//    public async Task<FeeHeadResponse?> GetFeeHeadByIdAsync(
//        int id,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.GetAsync<FeeHeadResponse>(
//            $"api/fees/heads/{id}",
//            cancellationToken);

//        if (IsNotFound(response))
//            return null;

//        EnsureSuccess(response);
//        return response.ServerResponse;
//    }

//    public async Task<FeeHeadResponse> CreateFeeHeadAsync(
//        CreateFeeHeadRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PostAsync<CreateFeeHeadRequest, FeeHeadResponse>(
//            "api/fees/heads",
//            request,
//            cancellationToken);

//        EnsureSuccess(response);

//        if (response.ServerResponse is null)
//            throw new Exception("Fee head create response was empty.");

//        return response.ServerResponse;
//    }

//    public async Task UpdateFeeHeadAsync(
//        int id,
//        UpdateFeeHeadRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PutAsync<UpdateFeeHeadRequest, bool>(
//            $"api/fees/heads/{id}",
//            request,
//            cancellationToken);

//        EnsureSuccess(response);
//    }

//    public async Task DeleteFeeHeadAsync(
//        int id,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.DeleteAsync(
//            $"api/fees/heads/{id}",
//            cancellationToken);

//        EnsureDeleteSuccess(response);
//    }

//    public async Task<List<FeeStructureResponse>> GetFeeStructuresAsync(
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.GetAsync<List<FeeStructureResponse>>(
//            "api/fees/structures",
//            cancellationToken);

//        EnsureSuccess(response);
//        return response.ServerResponse ?? new List<FeeStructureResponse>();
//    }

//    public async Task<FeeStructureResponse?> GetFeeStructureByIdAsync(
//        int id,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.GetAsync<FeeStructureResponse>(
//            $"api/fees/structures/{id}",
//            cancellationToken);

//        if (IsNotFound(response))
//            return null;

//        EnsureSuccess(response);
//        return response.ServerResponse;
//    }

//    public async Task<FeeStructureResponse> CreateFeeStructureAsync(
//        CreateFeeStructureRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PostAsync<CreateFeeStructureRequest, FeeStructureResponse>(
//            "api/fees/structures",
//            request,
//            cancellationToken);

//        EnsureSuccess(response);

//        if (response.ServerResponse is null)
//            throw new Exception("Fee structure create response was empty.");

//        return response.ServerResponse;
//    }

//    public async Task UpdateFeeStructureAsync(
//        int id,
//        UpdateFeeStructureRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PutAsync<UpdateFeeStructureRequest, bool>(
//            $"api/fees/structures/{id}",
//            request,
//            cancellationToken);

//        EnsureSuccess(response);
//    }

//    public async Task DeleteFeeStructureAsync(
//        int id,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.DeleteAsync(
//            $"api/fees/structures/{id}",
//            cancellationToken);

//        EnsureDeleteSuccess(response);
//    }

//    public async Task<List<StudentChargeResponse>> GetStudentChargesAsync(
//        int studentId,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.GetAsync<List<StudentChargeResponse>>(
//            $"api/fees/charges/student/{studentId}",
//            cancellationToken);

//        EnsureSuccess(response);
//        return response.ServerResponse ?? new List<StudentChargeResponse>();
//    }

//    public async Task<List<FeeReceiptResponse>> GetStudentReceiptsAsync(
//        int studentId,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.GetAsync<List<FeeReceiptResponse>>(
//            $"api/fees/receipts/student/{studentId}",
//            cancellationToken);

//        EnsureSuccess(response);
//        return response.ServerResponse ?? new List<FeeReceiptResponse>();
//    }

//    public async Task<StudentFeeAssignmentResponse?> GetAssignmentAsync(
//        int admissionId,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.GetAsync<StudentFeeAssignmentResponse>(
//            $"api/fees/assignments/admission/{admissionId}",
//            cancellationToken);

//        if (IsNotFound(response))
//            return null;

//        EnsureSuccess(response);
//        return response.ServerResponse;
//    }

//    public async Task<StudentFeeAssignmentResponse> AssignFeeStructureAsync(
//        CreateStudentFeeAssignmentRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PostAsync<CreateStudentFeeAssignmentRequest, StudentFeeAssignmentResponse>(
//            "api/fees/assignments",
//            request,
//            cancellationToken);

//        EnsureSuccess(response);

//        if (response.ServerResponse is null)
//            throw new Exception("Assignment response was empty.");

//        return response.ServerResponse;
//    }

//    public async Task<StudentFeeAssignmentResponse> UpdateAssignmentAsync(
//        int assignmentId,
//        UpdateStudentFeeAssignmentRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PutAsync<UpdateStudentFeeAssignmentRequest, StudentFeeAssignmentResponse>(
//            $"api/fees/assignments/{assignmentId}",
//            request,
//            cancellationToken);

//        EnsureSuccess(response);

//        if (response.ServerResponse is null)
//            throw new Exception("Assignment update response was empty.");

//        return response.ServerResponse;
//    }

//    public async Task DeleteAssignmentAsync(
//        int assignmentId,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.DeleteAsync(
//            $"api/fees/assignments/{assignmentId}",
//            cancellationToken);

//        EnsureDeleteSuccess(response);
//    }

//    public async Task<List<StudentChargeResponse>> GenerateChargesAsync(
//        int assignmentId,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PostAsync<object, List<StudentChargeResponse>>(
//            $"api/fees/assignments/{assignmentId}/generate-charges",
//            new { },
//            cancellationToken);

//        EnsureSuccess(response);
//        return response.ServerResponse ?? new List<StudentChargeResponse>();
//    }

//    public async Task<FeeReceiptResponse> CollectFeeAsync(
//        CreateFeeReceiptRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PostAsync<CreateFeeReceiptRequest, FeeReceiptResponse>(
//            "api/fees/receipts",
//            request,
//            cancellationToken);

//        EnsureSuccess(response);

//        if (response.ServerResponse is null)
//            throw new Exception("Fee receipt response was empty.");

//        return response.ServerResponse;
//    }

//    public async Task CancelReceiptAsync(
//        int receiptId,
//        string? reason = null,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PostAsync<CancelFeeReceiptRequest, object>(
//            $"api/fees/receipts/{receiptId}/cancel",
//            new CancelFeeReceiptRequest { Reason = reason },
//            cancellationToken);

//        EnsureSuccess(response);
//    }

//    public async Task CancelChargeAsync(
//        int chargeId,
//        CancellationToken cancellationToken = default)
//    {
//        var response = await _httpService.PutAsync<object, bool>(
//            $"api/fees/charges/{chargeId}/cancel",
//            new { },
//            cancellationToken);

//        EnsureSuccess(response);
//    }

//    private static bool IsNotFound<T>(ServerResponseHelper<T> response)
//    {
//        return response.ResponseMessage?.StatusCode == System.Net.HttpStatusCode.NotFound;
//    }

//    private static void EnsureSuccess<T>(ServerResponseHelper<T> response)
//    {
//        if (response.IsSuccess || response.ResponseMessage?.IsSuccessStatusCode == true)
//            return;

//        var message = string.IsNullOrWhiteSpace(response.Message)
//            ? $"Request failed with status code {(int)(response.ResponseMessage?.StatusCode ?? 0)}."
//            : response.Message;

//        throw new Exception(message);
//    }

//    private static void EnsureDeleteSuccess(ServerResponseHelper<object> response)
//    {
//        if (response.IsSuccess || response.ResponseMessage?.IsSuccessStatusCode == true)
//            return;

//        var message = string.IsNullOrWhiteSpace(response.Message)
//            ? $"Request failed with status code {(int)(response.ResponseMessage?.StatusCode ?? 0)}."
//            : response.Message;

//        throw new Exception(message);
//    }
//}




using System.Text.Json;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.Fees;

public sealed class FeeWebRepository : IFeeWebRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpService _httpService;

    public FeeWebRepository(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<List<FeeHeadResponse>> GetFeeHeadsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<List<FeeHeadResponse>>(
            "api/fees/heads",
            cancellationToken);

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync(response, new List<FeeHeadResponse>());
    }

    public async Task<FeeHeadResponse?> GetFeeHeadByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<FeeHeadResponse>(
            $"api/fees/heads/{id}",
            cancellationToken);

        if (IsNotFound(response))
            return null;

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync<FeeHeadResponse?>(response, null);
    }

    public async Task<FeeHeadResponse> CreateFeeHeadAsync(
        CreateFeeHeadRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PostAsync<CreateFeeHeadRequest, FeeHeadResponse>(
            "api/fees/heads",
            request,
            cancellationToken);

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync<FeeHeadResponse?>(response, null);
        if (payload is null)
            throw new Exception("Fee head create response was empty.");

        return payload;
    }

    public async Task UpdateFeeHeadAsync(
        int id,
        UpdateFeeHeadRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PutAsync<UpdateFeeHeadRequest, bool>(
            $"api/fees/heads/{id}",
            request,
            cancellationToken);

        EnsureSuccess(response);
    }

    public async Task DeleteFeeHeadAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.DeleteAsync(
            $"api/fees/heads/{id}",
            cancellationToken);

        EnsureDeleteSuccess(response);
    }

    public async Task<List<FeeStructureResponse>> GetFeeStructuresAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<List<FeeStructureResponse>>(
            "api/fees/structures",
            cancellationToken);

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync(response, new List<FeeStructureResponse>());
    }

    public async Task<FeeStructureResponse?> GetFeeStructureByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<FeeStructureResponse>(
            $"api/fees/structures/{id}",
            cancellationToken);

        if (IsNotFound(response))
            return null;

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync<FeeStructureResponse?>(response, null);
    }

    public async Task<FeeStructureResponse> CreateFeeStructureAsync(
        CreateFeeStructureRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PostAsync<CreateFeeStructureRequest, FeeStructureResponse>(
            "api/fees/structures",
            request,
            cancellationToken);

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync<FeeStructureResponse?>(response, null);
        if (payload is null)
            throw new Exception("Fee structure create response was empty.");

        return payload;
    }

    public async Task UpdateFeeStructureAsync(
        int id,
        UpdateFeeStructureRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PutAsync<UpdateFeeStructureRequest, bool>(
            $"api/fees/structures/{id}",
            request,
            cancellationToken);

        EnsureSuccess(response);
    }

    public async Task DeleteFeeStructureAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.DeleteAsync(
            $"api/fees/structures/{id}",
            cancellationToken);

        EnsureDeleteSuccess(response);
    }

    public async Task<List<StudentChargeResponse>> GetStudentChargesAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<List<StudentChargeResponse>>(
            $"api/fees/charges/student/{studentId}",
            cancellationToken);

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync(response, new List<StudentChargeResponse>());
    }

    public async Task<List<FeeReceiptResponse>> GetStudentReceiptsAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<List<FeeReceiptResponse>>(
            $"api/fees/receipts/student/{studentId}",
            cancellationToken);

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync(response, new List<FeeReceiptResponse>());
    }

    public async Task<StudentFeeAssignmentResponse?> GetAssignmentAsync(
        int admissionId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<StudentFeeAssignmentResponse>(
            $"api/fees/assignments/admission/{admissionId}",
            cancellationToken);

        if (IsNotFound(response))
            return null;

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync<StudentFeeAssignmentResponse?>(response, null);
    }

    public async Task<StudentFeeAssignmentResponse> AssignFeeStructureAsync(
        CreateStudentFeeAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PostAsync<CreateStudentFeeAssignmentRequest, StudentFeeAssignmentResponse>(
            "api/fees/assignments",
            request,
            cancellationToken);

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync<StudentFeeAssignmentResponse?>(response, null);

        if (payload is not null)
            return payload;

        // Final fallback: fetch by admission after successful assign
        var assignment = await GetAssignmentAsync(request.StudentAdmissionId, cancellationToken);
        if (assignment is null)
            throw new Exception("Assignment response was empty and assignment could not be reloaded.");

        return assignment;
    }

    public async Task<StudentFeeAssignmentResponse> UpdateAssignmentAsync(
        int assignmentId,
        UpdateStudentFeeAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PutAsync<UpdateStudentFeeAssignmentRequest, StudentFeeAssignmentResponse>(
            $"api/fees/assignments/{assignmentId}",
            request,
            cancellationToken);

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync<StudentFeeAssignmentResponse?>(response, null);
        if (payload is null)
            throw new Exception("Assignment update response was empty.");

        return payload;
    }

    public async Task DeleteAssignmentAsync(
        int assignmentId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.DeleteAsync(
            $"api/fees/assignments/{assignmentId}",
            cancellationToken);

        EnsureDeleteSuccess(response);
    }

    public async Task<List<StudentChargeResponse>> GenerateChargesAsync(
        int assignmentId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PostAsync<object, List<StudentChargeResponse>>(
            $"api/fees/assignments/{assignmentId}/generate-charges",
            new { },
            cancellationToken);

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync(response, new List<StudentChargeResponse>());
    }

    public async Task<FeeReceiptResponse> CollectFeeAsync(
        CreateFeeReceiptRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PostAsync<CreateFeeReceiptRequest, FeeReceiptResponse>(
            "api/fees/receipts",
            request,
            cancellationToken);

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync<FeeReceiptResponse?>(response, null);
        if (payload is null)
            throw new Exception("Fee receipt response was empty.");

        return payload;
    }

    public async Task CancelReceiptAsync(
        int receiptId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PostAsync<CancelFeeReceiptRequest, object>(
            $"api/fees/receipts/{receiptId}/cancel",
            new CancelFeeReceiptRequest { Reason = reason },
            cancellationToken);

        EnsureSuccess(response);
    }

    public async Task CancelChargeAsync(
        int chargeId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PutAsync<object, bool>(
            $"api/fees/charges/{chargeId}/cancel",
            new { },
            cancellationToken);

        EnsureSuccess(response);
    }

    private static bool IsNotFound<T>(ServerResponseHelper<T> response)
    {
        return response.ResponseMessage?.StatusCode == System.Net.HttpStatusCode.NotFound;
    }

    private static void EnsureSuccess<T>(ServerResponseHelper<T> response)
    {
        if (response.IsSuccess || response.ResponseMessage?.IsSuccessStatusCode == true)
            return;

        var message = string.IsNullOrWhiteSpace(response.Message)
            ? $"Request failed with status code {(int)(response.ResponseMessage?.StatusCode ?? 0)}."
            : response.Message;

        throw new Exception(message);
    }

    private static void EnsureDeleteSuccess(ServerResponseHelper<object> response)
    {
        if (response.IsSuccess || response.ResponseMessage?.IsSuccessStatusCode == true)
            return;

        var message = string.IsNullOrWhiteSpace(response.Message)
            ? $"Request failed with status code {(int)(response.ResponseMessage?.StatusCode ?? 0)}."
            : response.Message;

        throw new Exception(message);
    }

    private static async Task<T> GetPayloadOrFallbackAsync<T>(
        ServerResponseHelper<T> response,
        T fallback)
    {
        if (response.ServerResponse is not null)
            return response.ServerResponse;

        if (response.ResponseMessage is null || !response.ResponseMessage.IsSuccessStatusCode)
            return fallback;

        var raw = await response.ResponseMessage.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(raw))
            return fallback;

        try
        {
            // 1) Direct payload
            var direct = JsonSerializer.Deserialize<T>(raw, JsonOptions);
            if (direct is not null)
                return direct;
        }
        catch
        {
            // ignore and try wrapped shape
        }

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            // 2) Wrapped payloads
            if (TryGetProperty(root, "ServerResponse", out var serverResponseElement) ||
                TryGetProperty(root, "serverResponse", out serverResponseElement) ||
                TryGetProperty(root, "Data", out serverResponseElement) ||
                TryGetProperty(root, "data", out serverResponseElement))
            {
                var wrapped = serverResponseElement.Deserialize<T>(JsonOptions);
                if (wrapped is not null)
                    return wrapped;
            }
        }
        catch
        {
            // ignore and return fallback
        }

        return fallback;
    }

    private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        foreach (var prop in element.EnumerateObject())
        {
            if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = prop.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}