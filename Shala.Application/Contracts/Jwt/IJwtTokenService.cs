using Shala.Domain.Entities.Identity;

namespace Shala.Application.Contracts.Jwt;

public interface IJwtTokenService
{
    Task<string> GeneratePlatformTokenAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default);

    Task<string> GenerateTenantTokenAsync(
        ApplicationUser user,
        string role,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);
}