using System.Text.Json;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.PlatformRepo;

public class BranchRepository : IBranchRepository
{
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BranchRepository(HttpClient httpClient, ApiSession session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task<List<BranchResponse>> GetAllAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<ApiEnvelope<List<BranchResponse>>>(
            $"api/platform/tenants/{tenantId}/branches",
            cancellationToken);

        return response?.Data ?? new List<BranchResponse>();
    }

    public async Task<BranchResponse?> GetByIdAsync(int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<ApiEnvelope<BranchResponse>>(
            $"api/platform/tenants/{tenantId}/branches/{branchId}",
            cancellationToken);

        return response?.Data;
    }

    public async Task<BranchResponse?> CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TenantId <= 0)
        {
            if (!_session.TenantId.HasValue || _session.TenantId.Value <= 0)
                throw new InvalidOperationException("Tenant session is missing.");

            request.TenantId = _session.TenantId.Value;
        }

        var httpResponse = await _httpClient.PostAsJsonAsync(
            $"api/platform/tenants/{request.TenantId}/branches",
            request,
            cancellationToken);

        var raw = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        ApiEnvelope<BranchResponse>? response = null;
        if (!string.IsNullOrWhiteSpace(raw))
        {
            response = JsonSerializer.Deserialize<ApiEnvelope<BranchResponse>>(raw, _jsonOptions);
        }

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response?.Message ?? "Branch creation failed.");

        return response?.Data;
    }

    public async Task<BranchResponse?> UpdateAsync(int tenantId, int branchId, UpdateBranchRequest request, CancellationToken cancellationToken = default)
    {
        var httpResponse = await _httpClient.PutAsJsonAsync(
            $"api/platform/tenants/{tenantId}/branches/{branchId}",
            request,
            cancellationToken);

        var raw = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        ApiEnvelope<BranchResponse>? response = null;
        if (!string.IsNullOrWhiteSpace(raw))
        {
            response = JsonSerializer.Deserialize<ApiEnvelope<BranchResponse>>(raw, _jsonOptions);
        }

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response?.Message ?? "Branch update failed.");

        return response?.Data;
    }

    public async Task<bool> DeleteAsync(int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        var httpResponse = await _httpClient.DeleteAsync(
            $"api/platform/tenants/{tenantId}/branches/{branchId}",
            cancellationToken);

        var raw = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        ApiEnvelope<bool>? response = null;
        if (!string.IsNullOrWhiteSpace(raw))
        {
            response = JsonSerializer.Deserialize<ApiEnvelope<bool>>(raw, _jsonOptions);
        }

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response?.Message ?? "Branch deletion failed.");

        return response?.Data ?? false;
    }

    private sealed class ApiEnvelope<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}