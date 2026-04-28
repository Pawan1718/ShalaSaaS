using Microsoft.AspNetCore.Identity;
using Shala.Application.Contracts.Jwt;
using Shala.Application.Features.Platform;
using Shala.Domain.Entities.Identity;
using Shala.Shared.Requests.Identity;
using Shala.Shared.Responses.Identity;

namespace Shala.Application.Features.Identity;

public sealed class PlatformAuthService : IPlatformAuthService
{
    private const string InvalidLoginMessage = "Invalid email or password.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public PlatformAuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<(bool Success, PlatformLoginResponse? Data, string Message)> LoginAsync(
        PlatformLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            return (false, null, InvalidLoginMessage);

        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return (false, null, InvalidLoginMessage);
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !user.IsActive)
            return (false, null, InvalidLoginMessage);

        var roles = await _userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Any(x =>
            string.Equals(x, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

        if (!isSuperAdmin)
            return (false, null, InvalidLoginMessage);

        if (await _userManager.IsLockedOutAsync(user))
            return (false, null, InvalidLoginMessage);

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!signInResult.Succeeded)
            return (false, null, InvalidLoginMessage);

        var token = await _jwtTokenService.GeneratePlatformTokenAsync(user, cancellationToken);

        var response = new PlatformLoginResponse
        {
            Token = token,
            Email = user.Email ?? string.Empty,
            Role = "SuperAdmin",
            FullName = user.FullName,
            TenantId = null,
            TenantName = null,
            BranchId = null,
            BranchName = null,
            BranchCode = null,
            RequiresBranchSelection = false,
            BranchOptions = new List<LoginBranchOption>()
        };

        return (true, response, "Login successful.");
    }
}