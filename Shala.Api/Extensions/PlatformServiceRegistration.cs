using Shala.Application.Common;
using Shala.Application.Features.Platform;
using Shala.Application.Repositories.Platform;
using Shala.Infrastructure.Repositories.Platform;

namespace Shala.Api.Extensions;

public static class PlatformServiceRegistration
{
    public static IServiceCollection AddPlatformServices(this IServiceCollection services)
    {
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<ITenantProvisionRepository, TenantProvisionRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IBranchCodeGenerator, BranchCodeGenerator>();

        services.AddScoped<IPlatformDashboardService, PlatformDashboardService>();
        services.AddScoped<ITenantProvisionService, TenantProvisionService>();
        services.AddScoped<IBranchService, BranchService>();

        return services;
    }
}