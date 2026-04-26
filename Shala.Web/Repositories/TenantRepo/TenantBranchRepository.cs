using System.Net.Http.Json;
using Shala.Shared.Responses.Tenant;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.TenantRepo;

public class TenantBranchRepository : ITenantBranchRepository
{
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;

    public TenantBranchRepository(HttpClient httpClient, ApiSession session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task<List<TenantBranchOptionDto>> GetMyBranchesAsync()
    {
        if (!string.IsNullOrWhiteSpace(_session.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
        }

        var result = await _httpClient.GetFromJsonAsync<List<TenantBranchOptionDto>>(
            "api/tenant/branches/my");

        return result ?? new List<TenantBranchOptionDto>();
    }
}