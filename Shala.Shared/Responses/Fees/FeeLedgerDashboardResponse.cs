using Shala.Shared.Common;

namespace Shala.Shared.Responses.Fees;

public sealed class FeeLedgerDashboardResponse
{
    public FeeLedgerDashboardSummaryResponse Summary { get; set; } = new();
    public PagedResult<FeeLedgerRowResponse> Entries { get; set; } = new();
}