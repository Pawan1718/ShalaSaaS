using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shala.Application.Features.Identity;
using Shala.Shared.Requests.Identity;

namespace Shala.Api.Controllers.Auth;

[Route("api/tenant/auth")]
[ApiController]
public sealed class TenantAuthController : ControllerBase
{
    private readonly ITenantAuthService _authService;

    public TenantAuthController(ITenantAuthService authService)
    {
        _authService = authService;
    }

    [EnableRateLimiting("tenant-login")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] TenantLoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (!result.Success)
            return Unauthorized(new { success = false, message = result.Message });

        return Ok(new
        {
            success = true,
            message = result.Message,
            data = result.Data
        });
    }
}