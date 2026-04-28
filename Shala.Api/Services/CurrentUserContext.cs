using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shala.Application.Contracts;
using AppClaimConstants = Shala.Application.Contracts.ClaimConstants;

namespace Shala.Api.Services;

public sealed class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("No authenticated user context available.");

    public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

    public string? UserId =>
        User.FindFirstValue(AppClaimConstants.UserId)
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email =>
        User.FindFirstValue(AppClaimConstants.Email)
        ?? User.FindFirstValue(ClaimTypes.Email);

    public string? FullName =>
        User.FindFirstValue(AppClaimConstants.FullName)
        ?? User.FindFirstValue(AppClaimConstants.LegacyFullName);

    public string? Role =>
        User.FindFirstValue(AppClaimConstants.Role)
        ?? User.FindFirstValue(ClaimTypes.Role);

    public int? TenantId =>
        TryGetIntClaim(AppClaimConstants.TenantId, AppClaimConstants.LegacyTenantId);

    public int? BranchId =>
        TryGetIntClaim(AppClaimConstants.BranchId, AppClaimConstants.LegacyBranchId);

    public bool IsPlatformAdmin =>
        string.Equals(Role, "SuperAdmin", StringComparison.OrdinalIgnoreCase);

    public int GetRequiredTenantId()
    {
        if (!TenantId.HasValue || TenantId.Value <= 0)
            throw new UnauthorizedAccessException("Tenant claim is missing or invalid.");

        return TenantId.Value;
    }

    public int GetRequiredBranchId()
    {
        if (!BranchId.HasValue || BranchId.Value <= 0)
            throw new UnauthorizedAccessException("Branch claim is missing or invalid.");

        return BranchId.Value;
    }

    public string GetRequiredUserId()
    {
        if (string.IsNullOrWhiteSpace(UserId))
            throw new UnauthorizedAccessException("User claim is missing or invalid.");

        return UserId;
    }

    private int? TryGetIntClaim(params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var raw = User.FindFirstValue(claimType);

            if (!string.IsNullOrWhiteSpace(raw) &&
                int.TryParse(raw, out var value) &&
                value > 0)
            {
                return value;
            }
        }

        return null;
    }
}