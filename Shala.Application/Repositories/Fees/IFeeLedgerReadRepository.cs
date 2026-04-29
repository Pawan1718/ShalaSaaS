using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Repositories.Fees;

public interface IFeeLedgerReadRepository
{
    Task<FeeLedgerDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        FeeLedgerDashboardRequest request,
        CancellationToken cancellationToken = default);
}