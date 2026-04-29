using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Tenant;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.TenantRepo;

public sealed class TenantDashboardWebRepository : ITenantDashboardWebRepository
{
    private readonly IHttpService _httpService;

    public TenantDashboardWebRepository(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public Task<ServerResponseHelper<TenantDashboardResponse>> GetAsync(
        TenantDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<TenantDashboardRequest, TenantDashboardResponse>(
            "api/tenant/dashboard",
            request,
            cancellationToken);
    }
}