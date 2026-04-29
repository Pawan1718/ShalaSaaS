using Shala.Shared.Enums;

namespace Shala.Shared.Requests.Tenant
{

    public class CreateTenantUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public AppRole Role { get; set; }

        public int DefaultBranchId { get; set; }
        public List<int> AllowedBranchIds { get; set; } = new();
    }

}

