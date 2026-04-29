using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public interface IFeeLedgerService
{
    Task<FeeLedgerDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        FeeLedgerDashboardRequest request,
        CancellationToken cancellationToken = default);
}