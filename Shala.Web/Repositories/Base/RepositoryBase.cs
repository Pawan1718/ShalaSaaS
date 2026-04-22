using System.Net.Http.Headers;
using System.Text.Json;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.Base;

public abstract class RepositoryBase
{
    protected readonly HttpClient HttpClient;
    private readonly ApiSession _session;

    protected RepositoryBase(HttpClient httpClient, ApiSession session)
    {
        HttpClient = httpClient;
        _session = session;
    }

    protected async Task EnsureAuthAsync()
    {
        await _session.InitializeAsync();

        if (!string.IsNullOrWhiteSpace(_session.Token))
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _session.Token);
        }
    }

    protected static async Task<T?> ReadApiResponse<T>(
        HttpResponseMessage response,
        string defaultErrorMessage)
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