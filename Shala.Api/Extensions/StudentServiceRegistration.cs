using Shala.Application.Features.Academics;
using Shala.Application.Features.Registration;
using Shala.Application.Features.StudentDocument;
using Shala.Application.Features.Students;
using Shala.Application.Features.TenantConfig;
using Shala.Application.Repositories.Academics;
using Shala.Application.Repositories.Registration;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Application.Repositories.Students;
using Shala.Application.Repositories.TenantConfig;
using Shala.Infrastructure.Repositories.Academics;
using Shala.Infrastructure.Repositories.Registration;
using Shala.Infrastructure.Repositories.StudentDocumentRepo;
using Shala.Infrastructure.Repositories.Students;
using Shala.Infrastructure.Repositories.TenantConfig;

namespace Shala.Api.Extensions;

public static class StudentServiceRegistration
{
    public static IServiceCollection AddStudentServices(this IServiceCollection services)
    {
        services.AddScoped<IAcademicYearSettingRepository, AcademicYearSettingRepository>();
        services.AddScoped<IRollNumberSettingRepository, RollNumberSettingRepository>();
        services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
        services.AddScoped<IAcademicClassRepository, AcademicClassRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IStudentAdmissionRepository, StudentAdmissionRepository>();

        services.AddScoped<IStudentAdmissionService, StudentAdmissionService>();
        services.AddScoped<IRollNumberSettingService, RollNumberSettingService>();
        services.AddScoped<IRollNumberGeneratorService, RollNumberGeneratorService>();
        services.AddScoped<IAcademicYearService, AcademicYearService>();
        services.AddScoped<IAcademicClassService, AcademicClassService>();
        services.AddScoped<ISectionService, SectionService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IStudentGuardianService, StudentGuardianService>();
        services.AddScoped<IStudentGuardianRepository, StudentGuardianRepository>();



        services.AddScoped<IRegistrationFeeService, RegistrationFeeService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IRegistrationFeeRepository, RegistrationFeeRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();



        services.AddScoped<IRegistrationFeeConfigurationRepository, RegistrationFeeConfigurationRepository>();
        services.AddScoped<IRegistrationReceiptConfigurationRepository, RegistrationReceiptConfigurationRepository>();
        services.AddScoped<IRegistrationProspectusConfigurationRepository, RegistrationProspectusConfigurationRepository>();

        services.AddScoped<IRegistrationFeeConfigurationService, RegistrationFeeConfigurationService>();
        services.AddScoped<IRegistrationReceiptConfigurationService, RegistrationReceiptConfigurationService>();
        services.AddScoped<IRegistrationProspectusConfigurationService, RegistrationProspectusConfigurationService>();

        services.AddScoped<IDocumentModelRepository, DocumentModelRepository>();
        services.AddScoped<IStudentDocumentRepository, StudentDocumentRepository>();

        services.AddScoped<IDocumentModelService, DocumentModelService>();
        services.AddScoped<IStudentDocumentService, StudentDocumentService>();

        return services;
    }
}