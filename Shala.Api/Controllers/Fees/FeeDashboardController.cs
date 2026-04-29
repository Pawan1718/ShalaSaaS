using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Fees;
using Shala.Shared.Common;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Api.Controllers.Fees;

[ApiController]
[Route("api/fees/dashboard")]
public sealed class FeeDashboardController : TenantApiControllerBase
{
    private readonly IFeeDashboardService _feeDashboardService;

    public FeeDashboardController(
        IFeeDashboardService feeDashboardService,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _feeDashboardService = feeDashboardService;
    }

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<FeeDashboardResponse>>> SearchAsync(
        [FromBody] FeeDashboardRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _feeDashboardService.GetDashboardAsync(
            TenantId,
            BranchId,
            request,
            cancellationToken);

        return Ok(ApiResponse<FeeDashboardResponse>.Ok(result));
    }
}