using Shala.Shared.Common;
using Shala.Shared.Responses.Platform;

namespace Shala.Application.Features.Platform;

public interface IPlatformDashboardService
{
    Task<ApiResponse<PlatformDashboardResponse>> GetSummaryAsync(
        CancellationToken cancellationToken = default);
}