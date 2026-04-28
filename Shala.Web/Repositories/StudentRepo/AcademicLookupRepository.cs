using System.Net.Http.Headers;
using System.Text.Json;
using Shala.Shared.Common;
using Shala.Shared.Responses.Students;
using Shala.Web.Repositories.Interfaces;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.Students;

public class AcademicLookupRepository : IAcademicLookupRepository
{
    private const string BaseRoute = "api/students/lookups";
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;

    public AcademicLookupRepository(HttpClient httpClient, ApiSession session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task<ApiResponse<List<LookupItemResponse>>?> GetAcademicYearsAsync(int tenantId, int branchId)
    {
        await EnsureAuthAsync();
        return await SendGetAsync($"{BaseRoute}/academic-years?tenantId={tenantId}&branchId={branchId}", "academic years");
    }

    public async Task<ApiResponse<List<LookupItemResponse>>?> GetClassesAsync(int tenantId, int branchId)
    {
        await EnsureAuthAsync();
        return await SendGetAsync($"{BaseRoute}/classes?tenantId={tenantId}&branchId={branchId}", "classes");
    }

    public async Task<ApiResponse<List<LookupItemResponse>>?> GetSectionsByClassAsync(int tenantId, int branchId, int classId)
    {
        await EnsureAuthAsync();
        return await SendGetAsync($"{BaseRoute}/sections?tenantId={tenantId}&branchId={branchId}&classId={classId}", "sections");
    }

    private async Task EnsureAuthAsync()
    {
        await _session.InitializeAsync();

        _httpClient.DefaultRequestHeaders.Remove("X-Branch-Id");
        _httpClient.DefaultRequestHeaders.Remove("X-All-Branches");

        if (!string.IsNullOrWhiteSpace(_session.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _session.Token);
        }

        if (_session.IsAllBranchesSelected)
        {
            _httpClient.DefaultRequestHeaders.Add("X-All-Branches", "true");
        }
        else if (_session.SelectedBranchId.HasValue)
        {
            _httpClient.DefaultRequestHeaders.Add("X-Branch-Id", _session.SelectedBranchId.Value.ToString());
        }
    }

    private async Task<ApiResponse<List<LookupItemResponse>>?> SendGetAsync(string url, string lookupName)
    {
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"{lookupName} lookup failed. Url: {url}, Status: {(int)response.StatusCode}, Body: {content}");

        if (string.IsNullOrWhiteSpace(content))
            throw new Exception($"{lookupName} lookup failed. Empty response body.");

        return JsonSerializer.Deserialize<ApiResponse<List<LookupItemResponse>>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}