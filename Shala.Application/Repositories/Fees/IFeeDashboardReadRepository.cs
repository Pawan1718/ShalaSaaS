using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Repositories.Fees;

public interface IFeeDashboardReadRepository
{
    Task<FeeDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        FeeDashboardRequest request,
        CancellationToken cancellationToken = default);
}