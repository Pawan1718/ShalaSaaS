using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Tenant;

namespace Shala.Application.Features.Tenant;

public interface ITenantDashboardService
{
    Task<TenantDashboardResponse> GetAsync(
        int tenantId,
        string userId,
        string role,
        TenantDashboardRequest request,
        CancellationToken cancellationToken = default);
}