using Microsoft.AspNetCore.RateLimiting;
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
using System.Text.Json;
using System.Threading.RateLimiting;

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

        builder.Services.AddJwtAuthentication(
            builder.Configuration,
            builder.Environment);

        builder.Services.AddAuthorization();

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.ContentType = "application/json";

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Too many login attempts. Please try again later."
                }, token);
            };

            // Platform login: LoginId/Email + IP
            options.AddPolicy("platform-login", httpContext =>
            {
                var ip = GetClientIp(httpContext);
                var body = ReadRawBody(httpContext);

                var loginKey = Normalize(
                    GetJsonValue(body, "Email") ??
                    GetJsonValue(body, "UserName") ??
                    GetJsonValue(body, "LoginId") ??
                    GetJsonValue(body, "Identifier") ??
                    GetJsonValue(body, "MobileNumber") ??
                    GetJsonValue(body, "PhoneNumber"));

                var key = $"platform-login|login:{loginKey}|ip:{ip}";

                return RateLimitPartition.GetFixedWindowLimiter(
                    key,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });

            // Tenant login: Tenant + Branch + LoginId + IP
            options.AddPolicy("tenant-login", httpContext =>
            {
                var ip = GetClientIp(httpContext);
                var body = ReadRawBody(httpContext);

                var tenantKey = Normalize(
                    GetJsonValue(body, "TenantId") ??
                    GetJsonValue(body, "TenantCode") ??
                    GetJsonValue(body, "TenantName") ??
                    GetJsonValue(body, "SchoolCode") ??
                    GetJsonValue(body, "SchoolName") ??
                    GetJsonValue(body, "Tenant"));

                var branchKey = Normalize(
                    GetJsonValue(body, "BranchId") ??
                    GetJsonValue(body, "BranchCode") ??
                    GetJsonValue(body, "BranchName") ??
                    GetJsonValue(body, "Branch"));

                var loginKey = Normalize(
                    GetJsonValue(body, "Email") ??
                    GetJsonValue(body, "UserName") ??
                    GetJsonValue(body, "LoginId") ??
                    GetJsonValue(body, "Identifier") ??
                    GetJsonValue(body, "MobileNumber") ??
                    GetJsonValue(body, "PhoneNumber") ??
                    GetJsonValue(body, "Mobile") ??
                    GetJsonValue(body, "UserId"));

                var key =
                    $"tenant-login|tenant:{tenantKey}|branch:{branchKey}|login:{loginKey}|ip:{ip}";

                return RateLimitPartition.GetFixedWindowLimiter(
                    key,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
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

        await app.RunAsync();
    }

    private static string Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "unknown"
            : value.Trim().ToLowerInvariant();
    }

    private static string GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(forwardedFor))
            return forwardedFor.Split(',')[0].Trim();

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static string ReadRawBody(HttpContext context)
    {
        if (!context.Request.HasJsonContentType())
            return string.Empty;

        context.Request.EnableBuffering();

        using var reader = new StreamReader(
            context.Request.Body,
            leaveOpen: true);

        var body = reader.ReadToEndAsync().GetAwaiter().GetResult();

        context.Request.Body.Position = 0;

        return body;
    }

    private static string? GetJsonValue(string json, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return null;

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    return prop.Value.ToString();
            }
        }
        catch
        {
            return null;
        }

        return null;
    }
}