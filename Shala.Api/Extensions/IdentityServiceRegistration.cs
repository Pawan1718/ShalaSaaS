using Microsoft.AspNetCore.Identity;
using Shala.Application.Contracts.Jwt;
using Shala.Application.Features.Identity;
using Shala.Application.Features.Platform;
using Shala.Domain.Entities.Identity;
using Shala.Infrastructure.Data;

namespace Shala.Api.Extensions;

public static class IdentityServiceRegistration
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPlatformAuthService, PlatformAuthService>();
        services.AddScoped<ITenantAuthService, TenantAuthService>();
        services.AddScoped<IUserService, UserService>();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        return services;
    }
}