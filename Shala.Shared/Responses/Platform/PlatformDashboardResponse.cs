namespace Shala.Shared.Responses.Platform
{
    public class PlatformDashboardResponse
    {
        public int TotalTenants { get; set; }
        public int ActiveTenants { get; set; }
        public int InactiveTenants { get; set; }

        public int TotalFreePlans { get; set; }
        public int TotalBasicPlans { get; set; }
        public int TotalStandardPlans { get; set; }
        public int TotalPremiumPlans { get; set; }
        public int TotalEnterprisePlans { get; set; }

        public int NewTenantsThisMonth { get; set; }
        public int NewTenantsLast30Days { get; set; }

        public List<DashboardChartItemResponse> MonthlyTenantGrowth { get; set; } = new();
        public List<DashboardChartItemResponse> SubscriptionBreakdown { get; set; } = new();
        public List<DashboardChartItemResponse> StatusBreakdown { get; set; } = new();
        public List<DashboardChartItemResponse> CategoryBreakdown { get; set; } = new();

        public List<RecentTenantDto> RecentTenants { get; set; } = new();
    }

    public class DashboardChartItemResponse
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    public class RecentTenantDto
    {
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BusinessCategory { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string SubscriptionPlan { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}