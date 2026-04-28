namespace Shala.Shared.Responses.Identity
{
    public class PlatformLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public int? TenantId { get; set; }
        public string? TenantName { get; set; }

        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCode { get; set; }

        public bool RequiresBranchSelection { get; set; }
        public List<LoginBranchOption> BranchOptions { get; set; } = new();
    }

    public class LoginBranchOption
    {
        public int BranchId { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public bool IsMainBranch { get; set; }
    }
}