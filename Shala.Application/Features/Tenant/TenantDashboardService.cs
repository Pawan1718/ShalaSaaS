using Shala.Application.Repositories.Tenant;
using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Tenant;

namespace Shala.Application.Features.Tenant;

public sealed class TenantDashboardService : ITenantDashboardService
{
    private readonly ITenantDashboardRepository _repository;

    public TenantDashboardService(ITenantDashboardRepository repository)
    {
        _repository = repository;
    }

    public Task<TenantDashboardResponse> GetAsync(
        int tenantId,
        string userId,
        string role,
        TenantDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        request ??= new TenantDashboardRequest();

        return _repository.GetAsync(
            tenantId,
            userId,
            role,
            request.BranchId,
            request.IsAllBranches,
            cancellationToken);
    }
}