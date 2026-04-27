

using System.Net;
using System.Text.Json;
using Shala.Shared.Common;
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

        var payload = await GetPayloadOrFallbackAsync(response, new List<StudentChargeResponse>());

        foreach (var charge in payload)
        {
            charge.ChargeLabel ??= string.Empty;
            charge.PeriodLabel ??= string.Empty;
        }

        return payload;
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

    public async Task<List<FeeReceiptResponse>> GetStudentReceiptsAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<List<FeeReceiptResponse>>(
            $"api/fees/receipts/student/{studentId}",
            cancellationToken);

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync(response, new List<FeeReceiptResponse>());

        foreach (var receipt in payload)
            NormalizeReceipt(receipt);

        return payload;
    }

    public async Task<FeeReceiptResponse?> GetReceiptByIdAsync(
        int receiptId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<FeeReceiptResponse>(
            $"api/fees/receipts/{receiptId}",
            cancellationToken);

        if (IsNotFound(response))
            return null;

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync<FeeReceiptResponse?>(response, null);
        if (payload is not null)
            NormalizeReceipt(payload);

        return payload;
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

        NormalizeReceipt(payload);
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
    public async Task<List<StudentFeeAssignmentResponse>> GetAssignmentsAsync(
    int admissionId,
    CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<List<StudentFeeAssignmentResponse>>(
            $"api/fees/assignments/admission/{admissionId}",
            cancellationToken);

        if (IsNotFound(response))
            return new List<StudentFeeAssignmentResponse>();

        EnsureSuccess(response);
        return await GetPayloadOrFallbackAsync(response, new List<StudentFeeAssignmentResponse>());
    }
    public async Task<StudentFeeAssignmentResponse?> GetAssignmentAsync(
     int admissionId,
     CancellationToken cancellationToken = default)
    {
        var assignments = await GetAssignmentsAsync(admissionId, cancellationToken);
        return assignments.FirstOrDefault(x => x.IsActive) ?? assignments.FirstOrDefault();
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
        if (payload is not null)
            return payload;

        var reloaded = await GetAssignmentAsync(request.StudentAdmissionId, cancellationToken);
        if (reloaded is null)
            throw new Exception("Assignment update response was empty and assignment could not be reloaded.");

        return reloaded;
    }

    public async Task<StudentFeeAssignmentResponse> UpdateAssignmentAsync(
        UpdateStudentFeeAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Id <= 0)
            throw new Exception("Assignment id is required for update.");

        return await UpdateAssignmentAsync(request.Id, request, cancellationToken);
    }

    public async Task<bool> CanModifyAssignmentAsync(
        int assignmentId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.GetAsync<ApiResponse<bool>>(
            $"api/fees/assignments/{assignmentId}/can-modify",
            cancellationToken);

        EnsureSuccess(response);

        if (response.ServerResponse is not null)
            return response.ServerResponse.Data;

        if (response.ResponseMessage is null || !response.ResponseMessage.IsSuccessStatusCode)
            return false;

        var raw = await response.ResponseMessage.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        try
        {
            var wrapped = JsonSerializer.Deserialize<ApiResponse<bool>>(raw, JsonOptions);
            if (wrapped is not null)
                return wrapped.Data;
        }
        catch
        {
        }

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (TryGetProperty(root, "data", out var dataElement))
            {
                if (dataElement.ValueKind == JsonValueKind.True || dataElement.ValueKind == JsonValueKind.False)
                    return dataElement.GetBoolean();

                var nested = dataElement.Deserialize<ApiResponse<bool>>(JsonOptions);
                if (nested is not null)
                    return nested.Data;
            }

            if (root.ValueKind == JsonValueKind.True || root.ValueKind == JsonValueKind.False)
                return root.GetBoolean();
        }
        catch
        {
        }

        return false;
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

        var payload = await GetPayloadOrFallbackAsync(response, new List<StudentChargeResponse>());

        foreach (var charge in payload)
        {
            charge.ChargeLabel ??= string.Empty;
            charge.PeriodLabel ??= string.Empty;
        }

        return payload;
    }

    public async Task<FeeDashboardResponse> GetDashboardAsync(
        FeeDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PostAsync<FeeDashboardRequest, FeeDashboardResponse>(
            "api/fees/dashboard/search",
            request,
            cancellationToken);

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync<FeeDashboardResponse?>(response, null);
        if (payload is null)
            throw new Exception("Fee dashboard response was empty.");

        return payload;
    }

    public async Task<FeeLedgerDashboardResponse> GetFeeLedgerDashboardAsync(
        FeeLedgerDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpService.PostAsync<FeeLedgerDashboardRequest, FeeLedgerDashboardResponse>(
            "api/fees/ledger/dashboard",
            request,
            cancellationToken);

        EnsureSuccess(response);

        var payload = await GetPayloadOrFallbackAsync<FeeLedgerDashboardResponse?>(response, null);
        if (payload is null)
            throw new Exception("Fee ledger dashboard response was empty.");

        return payload;
    }

    private static void NormalizeReceipt(FeeReceiptResponse receipt)
    {
        receipt.ReceiptNo ??= string.Empty;
        receipt.Allocations ??= new List<FeeReceiptAllocationResponse>();
    }

    private static bool IsNotFound<T>(ServerResponseHelper<T> response)
    {
        return response.ResponseMessage?.StatusCode == HttpStatusCode.NotFound;
    }

    private static void EnsureSuccess<T>(ServerResponseHelper<T> response)
    {
        if (response.IsSuccess || response.ResponseMessage?.IsSuccessStatusCode == true)
            return;

        var message = ExtractErrorMessage(response.Message, response.ResponseMessage?.StatusCode);
        throw new Exception(message);
    }

    private static void EnsureDeleteSuccess(ServerResponseHelper<object> response)
    {
        if (response.IsSuccess || response.ResponseMessage?.IsSuccessStatusCode == true)
            return;

        var message = ExtractErrorMessage(response.Message, response.ResponseMessage?.StatusCode);
        throw new Exception(message);
    }

    private static string ExtractErrorMessage(string? message, HttpStatusCode? statusCode)
    {
        if (!string.IsNullOrWhiteSpace(message))
            return message;

        if (statusCode.HasValue)
            return $"Request failed with status code {(int)statusCode.Value} ({statusCode.Value}).";

        return "Request failed.";
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
            var direct = JsonSerializer.Deserialize<T>(raw, JsonOptions);
            if (direct is not null)
                return direct;
        }
        catch
        {
        }

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (TryGetProperty(root, "ServerResponse", out var wrappedElement) ||
                TryGetProperty(root, "serverResponse", out wrappedElement) ||
                TryGetProperty(root, "Data", out wrappedElement) ||
                TryGetProperty(root, "data", out wrappedElement))
            {
                var wrapped = wrappedElement.Deserialize<T>(JsonOptions);
                if (wrapped is not null)
                    return wrapped;
            }
        }
        catch
        {
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