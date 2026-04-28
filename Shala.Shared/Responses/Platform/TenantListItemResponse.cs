namespace Shala.Shared.Responses.Platform
{
    public class TenantListItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BusinessCategory { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string SubscriptionPlan { get; set; } = string.Empty;
        public string? Subdomain { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int BranchCount { get; set; }
    }
}