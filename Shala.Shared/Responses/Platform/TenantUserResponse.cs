namespace Shala.Shared.Responses.Platform
{
    public class TenantUserResponse
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public int? DefaultBranchId { get; set; }
        public string DefaultBranchName { get; set; } = string.Empty;

        public bool HasAllBranchesAccess { get; set; }

        public List<int> AllowedBranchIds { get; set; } = new();
        public string AllowedBranchNames { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}