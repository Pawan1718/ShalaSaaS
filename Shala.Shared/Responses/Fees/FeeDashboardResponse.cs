using Shala.Shared.Common;

namespace Shala.Shared.Responses.Fees;

public sealed class FeeDashboardResponse
{
    public FeeDashboardSummaryResponse Summary { get; set; } = new();
    public PagedResult<FeeDashboardRowResponse> Students { get; set; } = new();
}