using Shala.Shared.Responses.Tenant;

namespace Shala.Application.Repositories.Tenant;

public interface ITenantDashboardRepository
{
    Task<TenantDashboardResponse> GetAsync(
        int tenantId,
        string userId,
        string role,
        int? requestedBranchId,
        bool isAllBranches,
        CancellationToken cancellationToken = default);
}