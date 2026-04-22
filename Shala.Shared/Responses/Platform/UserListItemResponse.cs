namespace Shala.Shared.Responses.Platform
{
    public class UserListItemResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public int? TenantId { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();


        public int? DefaultBranchId { get; set; }
        public string? DefaultBranchName { get; set; }
        public List<int> AllowedBranchIds { get; set; } = new();
        public List<string> AllowedBranchNames { get; set; } = new();
    }
}