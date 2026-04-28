namespace Shala.Shared.Requests.Platform
{
    public class CreateTenantRequest
    {
        public string SchoolName { get; set; } = string.Empty;
        public string BusinessCategory { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string SubscriptionPlan { get; set; } = "Basic";

        public string AdminFullName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
    }
}