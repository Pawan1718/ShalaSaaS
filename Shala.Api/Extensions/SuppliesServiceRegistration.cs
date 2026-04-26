using Shala.Application.Features.Supplies;
using Shala.Application.Repositories.Supplies;
using Shala.Infrastructure.Repositories.Supplies;

namespace Shala.Api.Extensions;

public static class SuppliesServiceRegistration
{
    public static IServiceCollection AddSuppliesServices(this IServiceCollection services)
    {
        services.AddScoped<ISupplyItemRepository, SupplyItemRepository>();
        services.AddScoped<IStudentSupplyIssueRepository, StudentSupplyIssueRepository>();
        services.AddScoped<IStudentSupplyPaymentRepository, StudentSupplyPaymentRepository>();
        services.AddScoped<ISupplyStockLedgerRepository, SupplyStockLedgerRepository>();

        services.AddScoped<IStudentSupplyService, StudentSupplyService>();

        return services;
    }
}