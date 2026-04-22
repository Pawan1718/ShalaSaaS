using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Responses.Academics;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.AcademicRepo;

public class AcademicClassRepository : IAcademicClassRepository
{
    private const string BaseRoute = "api/students/classes";
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;

    public AcademicClassRepository(HttpClient httpClient, ApiSession session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task<ApiResponse<List<AcademicClassListItemResponse>>?> GetAllAsync(int tenantId)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.GetAsync($"{BaseRoute}?tenantId={tenantId}");
        return await ReadApiResponse<ApiResponse<List<AcademicClassListItemResponse>>>(response, "Failed to load classes.");
    }

    public async Task<ApiResponse<AcademicClassListItemResponse>?> GetByIdAsync(int id)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.GetAsync($"{BaseRoute}/{id}");
        return await ReadApiResponse<ApiResponse<AcademicClassListItemResponse>>(response, "Failed to load class.");
    }

    public async Task<ApiResponse<int>?> CreateAsync(CreateAcademicClassRequest request)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.PostAsJsonAsync(BaseRoute, request);
        return await ReadApiResponse<ApiResponse<int>>(response, "Failed to create class.");
    }

    public async Task<ApiResponse<bool>?> UpdateAsync(UpdateAcademicClassRequest request)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.PutAsJsonAsync($"{BaseRoute}/{request.Id}", request);
        return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to update class.");
    }

    public async Task<ApiResponse<bool>?> DeleteAsync(int id)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.DeleteAsync($"{BaseRoute}/{id}");
        return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to delete class.");
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