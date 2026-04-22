using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shala.Application.Features.Platform;
using Shala.Shared.Common;
using Shala.Shared.Responses.Platform;

namespace Shala.Api.Controllers.Platform;

[Route("api/platform/dashboard")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public sealed class PlatformDashboardController : ControllerBase
{
    private readonly IPlatformDashboardService _dashboardService;

    public PlatformDashboardController(IPlatformDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<PlatformDashboardResponse>>> GetSummary(
     CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetSummaryAsync(cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}