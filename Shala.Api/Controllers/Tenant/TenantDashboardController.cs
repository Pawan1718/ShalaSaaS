using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Tenant;
using Shala.Shared.Common;
using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Tenant;

namespace Shala.Api.Controllers.Tenant;

[Route("api/tenant/dashboard")]
public sealed class TenantDashboardController : TenantApiControllerBase
{
    private readonly ITenantDashboardService _dashboardService;

    public TenantDashboardController(
        ITenantDashboardService dashboardService,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _dashboardService = dashboardService;
    }

    [HttpPost]
    public async Task<IActionResult> GetDashboard(
        [FromBody] TenantDashboardRequest request,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse<object>.Fail("User claim is missing."));

        var result = await _dashboardService.GetAsync(
            TenantId,
            userId,
            CurrentUser.Role ?? "TenantUser",
            request,
            cancellationToken);

        return Ok(ApiResponse<TenantDashboardResponse>.Ok(
            result,
            "Dashboard loaded successfully."));
    }
}