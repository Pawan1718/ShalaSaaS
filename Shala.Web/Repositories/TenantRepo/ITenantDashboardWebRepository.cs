using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Tenant;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.TenantRepo;

public interface ITenantDashboardWebRepository
{
    Task<ServerResponseHelper<TenantDashboardResponse>> GetAsync(
        TenantDashboardRequest request,
        CancellationToken cancellationToken = default);
}