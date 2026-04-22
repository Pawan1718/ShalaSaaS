using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Shala.Api.Extensions;
using Shala.Api.Middlewares;
using Shala.Api.Services;
using Shala.Application.Contracts;
using Shala.Application.Features.Settings;
using Shala.Application.Features.StudentDocument;
using Shala.Application.Features.Students;
using Shala.Application.Repositories.Settings;
using Shala.Infrastructure.Repositories.Settings;
using Shala.Infrastructure.Seed;
using Shala.Infrastructure.Storage;
using Shala.Web.Repositories.Settings;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        builder.Services.AddScoped<IAccessScopeValidator, AccessScopeValidator>();
        builder.Services.AddScoped<IBranchDocumentProfileRepository, BranchDocumentProfileRepository>();
        builder.Services.AddScoped<IBranchDocumentProfileService, BranchDocumentProfileService>();
        builder.Services.AddScoped<IStudentDocumentFileStorage, LocalStudentDocumentFileStorage>();

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddIdentityServices();
        builder.Services.AddPlatformServices();
        builder.Services.AddStudentServices();
        builder.Services.AddFeeServices();

        var jwtSection = builder.Configuration.GetSection("Jwt");
        var key = jwtSection["Key"];

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("JWT key is missing.");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("auth-login", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueLimit = 0;
            });
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            await SeedData.InitializeAsync(scope.ServiceProvider);
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}