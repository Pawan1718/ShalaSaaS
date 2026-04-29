using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shala.Application.Contracts;

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
        User.FindFirstValue(ClaimConstants.UserId)
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email =>
        User.FindFirstValue(ClaimConstants.Email)
        ?? User.FindFirstValue(ClaimTypes.Email);

    public string? FullName =>
        User.FindFirstValue(ClaimConstants.FullName)
        ?? User.FindFirstValue(ClaimConstants.LegacyFullName);

    public string? Role =>
        User.FindFirstValue(ClaimConstants.Role)
        ?? User.FindFirstValue(ClaimTypes.Role);

    public int? TenantId =>
        TryGetIntClaim(ClaimConstants.TenantId, ClaimConstants.LegacyTenantId);

    public int? BranchId =>
        TryGetIntClaim(ClaimConstants.BranchId, ClaimConstants.LegacyBranchId);

    public int GetRequiredTenantId()
    {
        var tenantId = TenantId;

        if (!tenantId.HasValue || tenantId.Value <= 0)
            throw new UnauthorizedAccessException("Tenant claim is missing or invalid.");

        return tenantId.Value;
    }

    public int GetRequiredBranchId()
    {
        var branchId = BranchId;

        if (!branchId.HasValue || branchId.Value <= 0)
            throw new UnauthorizedAccessException("Branch claim is missing or invalid.");

        return branchId.Value;
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