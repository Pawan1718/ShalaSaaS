using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.AcademicRepo;

public class SectionRepository : ISectionRepository
{
    private const string BaseRoute = "api/students/sections";
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;

    public SectionRepository(HttpClient httpClient, ApiSession session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task<ApiResponse<List<SectionListItemResponse>>?> GetAllAsync(int tenantId, int branchId)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.GetAsync($"{BaseRoute}?tenantId={tenantId}&branchId={branchId}");
        return await ReadApiResponse<ApiResponse<List<SectionListItemResponse>>>(response, "Failed to load sections.");
    }

    public async Task<ApiResponse<SectionListItemResponse>?> GetByIdAsync(int id)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.GetAsync($"{BaseRoute}/{id}");
        return await ReadApiResponse<ApiResponse<SectionListItemResponse>>(response, "Failed to load section.");
    }

    public async Task<ApiResponse<int>?> CreateAsync(CreateSectionRequest request)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.PostAsJsonAsync(BaseRoute, request);
        return await ReadApiResponse<ApiResponse<int>>(response, "Failed to create section.");
    }

    public async Task<ApiResponse<bool>?> UpdateAsync(UpdateSectionRequest request)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.PutAsJsonAsync($"{BaseRoute}/{request.Id}", request);
        return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to update section.");
    }

    public async Task<ApiResponse<bool>?> DeleteAsync(int id)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.DeleteAsync($"{BaseRoute}/{id}");
        return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to delete section.");
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

    private static async Task<T?> ReadApiResponse<T>(HttpResponseMessage response, string defaultErrorMessage)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"{defaultErrorMessage} Status: {(int)response.StatusCode}, Body: {content}");

        if (string.IsNullOrWhiteSpace(content))
            throw new Exception($"{defaultErrorMessage} Empty response body.");

        return JsonSerializer.Deserialize<T>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}