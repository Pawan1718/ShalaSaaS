using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public interface IFeeDashboardService
{
    Task<FeeDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        FeeDashboardRequest request,
        CancellationToken cancellationToken = default);
}