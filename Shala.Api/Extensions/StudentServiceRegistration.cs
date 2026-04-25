using Shala.Application.Features.Academics;
using Shala.Application.Features.StudentDocument;
using Shala.Application.Features.Students;
using Shala.Application.Features.TenantConfig;
using Shala.Application.Repositories.Academics;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Application.Repositories.Students;
using Shala.Application.Repositories.TenantConfig;
using Shala.Infrastructure.Repositories.Academics;
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

        services.AddScoped<IDocumentModelRepository, DocumentModelRepository>();
        services.AddScoped<IStudentDocumentChecklistRepository, StudentDocumentChecklistRepository>();
        services.AddScoped<IDocumentModelService, DocumentModelService>();
        services.AddScoped<IStudentDocumentChecklistService, StudentDocumentChecklistService>();

        return services;
    }
}