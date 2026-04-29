using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shala.Application.Contracts;
using Shala.Domain.Entities.Identity;

namespace Shala.Application.Contracts.Jwt;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> GeneratePlatformTokenAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        var roles = new List<string> { "SuperAdmin" };
        var token = BuildToken(user, roles, tenantId: null, branchId: null);
        return Task.FromResult(token);
    }

    public Task<string> GenerateTenantTokenAsync(
        ApplicationUser user,
        string role,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        var roles = new List<string> { role };
        var token = BuildToken(user, roles, tenantId, branchId);
        return Task.FromResult(token);
    }

    private string BuildToken(
        ApplicationUser user,
        IList<string> roles,
        int? tenantId,
        int? branchId)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var key = jwtSection["Key"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("JWT key is missing in configuration.");

        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];

        if (string.IsNullOrWhiteSpace(issuer))
            throw new InvalidOperationException("JWT issuer is missing in configuration.");

        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("JWT audience is missing in configuration.");

        var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var minutes)
            ? minutes
            : 60;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimConstants.UserId, user.Id),
            new(ClaimConstants.Email, user.Email ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.FullName))
        {
            claims.Add(new Claim(ClaimConstants.FullName, user.FullName));
        }

        if (tenantId.HasValue && tenantId.Value > 0)
        {
            claims.Add(new Claim(ClaimConstants.TenantId, tenantId.Value.ToString()));
        }

        if (branchId.HasValue && branchId.Value > 0)
        {
            claims.Add(new Claim(ClaimConstants.BranchId, branchId.Value.ToString()));
        }

        foreach (var role in roles
                     .Where(static x => !string.IsNullOrWhiteSpace(x))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimConstants.Role, role));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}