using Shala.Application.Features.Fees;
using Shala.Application.Repositories.Fees;
using Shala.Infrastructure.Repositories.Fees;

namespace Shala.Api.Extensions;

public static class FeeServiceRegistration
{
    public static IServiceCollection AddFeeServices(this IServiceCollection services)
    {
        services.AddScoped<Shala.Application.Repositories.Fees.IFeeHeadRepository, Shala.Infrastructure.Repositories.Fees.FeeHeadRepository>();
        services.AddScoped<IFeeStructureRepository, FeeStructureRepository>();
        services.AddScoped<IStudentFeeAssignmentRepository, StudentFeeAssignmentRepository>();
        services.AddScoped<IStudentChargeRepository, StudentChargeRepository>();
        services.AddScoped<IFeeReceiptRepository, FeeReceiptRepository>();

        services.AddScoped<IFeeHeadService, FeeHeadService>();
        services.AddScoped<IFeeStructureService, FeeStructureService>();
        services.AddScoped<IStudentFeeAssignmentService, StudentFeeAssignmentService>();
        services.AddScoped<IStudentChargeService, StudentChargeService>();
        services.AddScoped<IFeeReceiptService, FeeReceiptService>();
        services.AddScoped<IFeeReceiptNumberGenerator, FeeReceiptNumberGenerator>();
        services.AddScoped<IFeeChargeGenerationService, FeeChargeGenerationService>();

        services.AddScoped<IFeeDashboardService, FeeDashboardService>();

        services.AddScoped<IFeeDashboardReadRepository, FeeDashboardReadRepository>();

        services.AddScoped<IFeeLedgerService, FeeLedgerService>();
        services.AddScoped<IFeeLedgerReadRepository, FeeLedgerReadRepository>();

        services.AddScoped<IFeeLedgerPostingService, FeeLedgerPostingService>();
        services.AddScoped<IFeeLedgerWriteRepository, FeeLedgerWriteRepository>();


        services.AddScoped<IFeeReceiptCounterRepository, FeeReceiptCounterRepository>();


        return services;
    }
}