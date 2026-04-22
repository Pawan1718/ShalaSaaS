using System.Net.Http.Headers;
using System.Text.Json;
using Shala.Shared.Common;
using Shala.Shared.Responses.Students;
using Shala.Web.Repositories.Interfaces;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.Students;

public class StudentLookupRepository : IStudentLookupRepository
{
    private const string BaseRoute = "api/students/lookups";
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;

    public StudentLookupRepository(HttpClient httpClient, ApiSession session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task<ApiResponse<List<LookupItemResponse>>?> GetAcademicYearsAsync(int tenantId)
    {
        await EnsureAuthAsync();
        return await SendGetAsync($"{BaseRoute}/academic-years?tenantId={tenantId}", "academic years");
    }

    public async Task<ApiResponse<List<LookupItemResponse>>?> GetClassesAsync(int tenantId)
    {
        await EnsureAuthAsync();
        return await SendGetAsync($"{BaseRoute}/classes?tenantId={tenantId}", "classes");
    }

    public async Task<ApiResponse<List<LookupItemResponse>>?> GetSectionsByClassAsync(int tenantId, int branchId, int classId)
    {
        await EnsureAuthAsync();
        return await SendGetAsync($"{BaseRoute}/sections?tenantId={tenantId}&branchId={branchId}&classId={classId}", "sections");
    }

    private async Task EnsureAuthAsync()
    {
        await _session.InitializeAsync();

        if (!string.IsNullOrWhiteSpace(_session.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _session.Token);
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