using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.AcademicRepo;

public class AcademicYearRepository : IAcademicYearRepository
{
    private const string BaseRoute = "api/students/academic-years";
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;

    public AcademicYearRepository(HttpClient httpClient, ApiSession session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task<ApiResponse<List<AcademicYearListItemResponse>>?> GetAllAsync(int tenantId)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.GetAsync($"{BaseRoute}?tenantId={tenantId}");
        return await ReadApiResponse<ApiResponse<List<AcademicYearListItemResponse>>>(response, "Failed to load academic years.");
    }

    public async Task<ApiResponse<AcademicYearListItemResponse>?> GetByIdAsync(int id)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.GetAsync($"{BaseRoute}/{id}");
        return await ReadApiResponse<ApiResponse<AcademicYearListItemResponse>>(response, "Failed to load academic year.");
    }

    public async Task<ApiResponse<int>?> CreateAsync(CreateAcademicYearRequest request)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.PostAsJsonAsync(BaseRoute, request);
        return await ReadApiResponse<ApiResponse<int>>(response, "Failed to create academic year.");
    }

    public async Task<ApiResponse<bool>?> UpdateAsync(UpdateAcademicYearRequest request)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.PutAsJsonAsync($"{BaseRoute}/{request.Id}", request);
        return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to update academic year.");
    }

    public async Task<ApiResponse<bool>?> DeleteAsync(int id)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.DeleteAsync($"{BaseRoute}/{id}");
        return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to delete academic year.");
    }


    public async Task<ApiResponse<bool>?> SetCurrentAsync(int id, int tenantId)
    {
        await EnsureAuthAsync();
        var response = await _httpClient.PostAsync($"{BaseRoute}/{id}/set-current?tenantId={tenantId}", null);
        return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to set current academic year.");
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