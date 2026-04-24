using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor.Services;
using Shala.Web.Components;
using Shala.Web.Repositories.AcademicRepo;
using Shala.Web.Repositories.AuthRepo;
using Shala.Web.Repositories.Fees;
using Shala.Web.Repositories.Interfaces;
using Shala.Web.Repositories.PlatformRepo;
using Shala.Web.Repositories.Registration;
using Shala.Web.Repositories.Registration.RegistrationConfig;
using Shala.Web.Repositories.Settings;
using Shala.Web.Repositories.StudentDocuments;
using Shala.Web.Repositories.StudentRepo;
using Shala.Web.Repositories.Students;
using Shala.Web.Repositories.Supplies;
using Shala.Web.Repositories.TenantRepo;
using Shala.Web.Services.Http;
using Shala.Web.Services.State;

namespace Shala.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddMudServices();

        var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
        if (string.IsNullOrWhiteSpace(apiBaseUrl))
            throw new InvalidOperationException("ApiSettings:BaseUrl is missing.");

        builder.Services.AddScoped(_ => new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl)
        });

        builder.Services.AddScoped<ProtectedSessionStorage>();
        builder.Services.AddScoped<ApiSession>();

        builder.Services.AddScoped<IHttpService, HttpService>();
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IPlatformDashboardRepository, PlatformDashboardRepository>();
        builder.Services.AddScoped<ITenantRepository, TenantRepository>();
        builder.Services.AddScoped<TenantUserRepository>();
        builder.Services.AddScoped<IBranchRepository, BranchRepository>();

        builder.Services.AddScoped<IStudentRepository, StudentRepository>();
        builder.Services.AddScoped<IStudentLookupRepository, StudentLookupRepository>();
        builder.Services.AddScoped<IStudentAdmissionRepository, StudentAdmissionRepository>();
        builder.Services.AddScoped<IStudentGuardianRepository, StudentGuardianRepository>();

        builder.Services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
        builder.Services.AddScoped<IAcademicClassRepository, AcademicClassRepository>();
        builder.Services.AddScoped<ISectionRepository, SectionRepository>();
        builder.Services.AddScoped<IRollNumberSettingRepository, RollNumberSettingRepository>();
        builder.Services.AddScoped<IRegistrationWebRepository, RegistrationWebRepository>();

        builder.Services.AddScoped<IRegistrationFeeConfigurationWebRepository, RegistrationFeeConfigurationWebRepository>();
        builder.Services.AddScoped<IRegistrationReceiptConfigurationWebRepository, RegistrationReceiptConfigurationWebRepository>();
        builder.Services.AddScoped<IRegistrationProspectusConfigurationWebRepository, RegistrationProspectusConfigurationWebRepository>();
        builder.Services.AddScoped<IRegistrationFeeHeadLookupWebRepository, RegistrationFeeHeadLookupWebRepository>();
        builder.Services.AddScoped<IBranchDocumentProfileWebRepository, BranchDocumentProfileWebRepository>();
        builder.Services.AddScoped<IFeeWebRepository, FeeWebRepository>();

        builder.Services.AddScoped<IDocumentModelWebRepository, DocumentModelWebRepository>();
        builder.Services.AddScoped<IStudentDocumentWebRepository, StudentDocumentWebRepository>();


        builder.Services.AddScoped<ISuppliesWebRepository, SuppliesWebRepository>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}