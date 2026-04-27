using Microsoft.AspNetCore.RateLimiting;
using Shala.Api.Extensions;
using Shala.Api.Services;
using Shala.Application.Contracts;
using Shala.Application.Features.Settings;
using Shala.Application.Repositories.Settings;
using Shala.Infrastructure.Repositories.Settings;
using Shala.Infrastructure.Seed;
using Shala.Infrastructure.Services;
using System.Text.Json;
using System.Threading.RateLimiting;

public partial class Program
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

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddIdentityServices();
        builder.Services.AddPlatformServices();
        builder.Services.AddStudentServices();
        builder.Services.AddFeeServices();
        builder.Services.AddSuppliesServices();

        builder.Services.AddScoped<IAdmissionNumberGenerator, AdmissionNumberGenerator>();

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

            options.AddPolicy("tenant-login", httpContext =>
            {
                var ip = GetClientIp(httpContext);
                var body = ReadRawBody(httpContext);

                var tenantKey = Normalize(GetJsonValue(body, "TenantId"));
                var branchKey = Normalize(GetJsonValue(body, "BranchId"));
                var loginKey = Normalize(GetJsonValue(body, "UserId"));

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

        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            await SeedData.InitializeAsync(scope.ServiceProvider);
            app.MapOpenApi();
        }
        else
        {
            app.UseHsts();
        }

        // ✅ GLOBAL EXCEPTION HANDLER
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionFeature =
                    context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

                var exception = exceptionFeature?.Error;

                context.Response.ContentType = "application/json";

                if (exception is UnauthorizedAccessException)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = exception.Message
                    });

                    return;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "An unexpected error occurred."
                });
            });
        });

        app.UseHttpsRedirection();

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

        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);

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
        catch { }

        return null;
    }
}