using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Infrastructure.Data;
using Shala.Infrastructure.Repositories;

namespace Shala.Api.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Shala.Api")));

        return services;
    }
}