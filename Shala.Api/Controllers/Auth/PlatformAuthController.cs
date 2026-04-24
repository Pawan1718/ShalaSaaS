using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shala.Application.Features.Platform;
using Shala.Shared.Requests.Identity;

namespace Shala.Api.Controllers.Auth;

[Route("api/platform/auth")]
[ApiController]
public sealed class PlatformAuthController : ControllerBase
{
    private readonly IPlatformAuthService _platformAuthService;

    public PlatformAuthController(IPlatformAuthService platformAuthService)
    {
        _platformAuthService = platformAuthService;
    }

    [EnableRateLimiting("platform-login")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] PlatformLoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _platformAuthService.LoginAsync(request, cancellationToken);

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