using Shala.Application.Repositories.Platform;
using Shala.Shared.Common;
using Shala.Shared.Responses.Platform;

namespace Shala.Application.Features.Platform;

public sealed class PlatformDashboardService : IPlatformDashboardService
{
    private readonly IPlatformRepository _platformRepository;

    public PlatformDashboardService(IPlatformRepository platformRepository)
    {
        _platformRepository = platformRepository;
    }

    public async Task<ApiResponse<PlatformDashboardResponse>> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var tenants = await _platformRepository.GetAllTenantsAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var last30Days = now.AddDays(-30);

        var response = new PlatformDashboardResponse
        {
            TotalTenants = tenants.Count,
            ActiveTenants = tenants.Count(x => x.IsActive),
            InactiveTenants = tenants.Count(x => !x.IsActive),

            TotalFreePlans = tenants.Count(x => x.SubscriptionPlan == "Free"),
            TotalBasicPlans = tenants.Count(x => x.SubscriptionPlan == "Basic"),
            TotalStandardPlans = tenants.Count(x => x.SubscriptionPlan == "Standard"),
            TotalPremiumPlans = tenants.Count(x => x.SubscriptionPlan == "Premium"),
            TotalEnterprisePlans = tenants.Count(x => x.SubscriptionPlan == "Enterprise"),

            NewTenantsThisMonth = tenants.Count(x => x.CreatedAt >= startOfMonth),
            NewTenantsLast30Days = tenants.Count(x => x.CreatedAt >= last30Days),

            StatusBreakdown = new List<DashboardChartItemResponse>
            {
                new() { Label = "Active", Value = tenants.Count(x => x.IsActive) },
                new() { Label = "Inactive", Value = tenants.Count(x => !x.IsActive) }
            },

            SubscriptionBreakdown = new List<DashboardChartItemResponse>
            {
                new() { Label = "Free", Value = tenants.Count(x => x.SubscriptionPlan == "Free") },
                new() { Label = "Basic", Value = tenants.Count(x => x.SubscriptionPlan == "Basic") },
                new() { Label = "Standard", Value = tenants.Count(x => x.SubscriptionPlan == "Standard") },
                new() { Label = "Premium", Value = tenants.Count(x => x.SubscriptionPlan == "Premium") },
                new() { Label = "Enterprise", Value = tenants.Count(x => x.SubscriptionPlan == "Enterprise") }
            },

            CategoryBreakdown = tenants
                .GroupBy(x => string.IsNullOrWhiteSpace(x.BusinessCategory) ? "Other" : x.BusinessCategory)
                .Select(g => new DashboardChartItemResponse
                {
                    Label = g.Key,
                    Value = g.Count()
                })
                .OrderByDescending(x => x.Value)
                .ToList(),

            RecentTenants = tenants
                .OrderByDescending(x => x.CreatedAt)
                .Take(8)
                .Select(x => new RecentTenantDto
                {
                    TenantId = x.Id,
                    Name = x.Name,
                    BusinessCategory = x.BusinessCategory ?? string.Empty,
                    Email = x.Email ?? string.Empty,
                    MobileNumber = x.MobileNumber ?? string.Empty,
                    SubscriptionPlan = x.SubscriptionPlan ?? string.Empty,
                    Subdomain = x.Subdomain ?? string.Empty,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToList()
        };

        response.MonthlyTenantGrowth = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var monthDate = startOfMonth.AddMonths(-5 + offset);
                var count = tenants.Count(x =>
                    x.CreatedAt.Year == monthDate.Year &&
                    x.CreatedAt.Month == monthDate.Month);

                return new DashboardChartItemResponse
                {
                    Label = monthDate.ToString("MMM yyyy"),
                    Value = count
                };
            })
            .ToList();

        return ApiResponse<PlatformDashboardResponse>.Ok(
            response,
            "Platform dashboard summary loaded successfully.");
    }
}