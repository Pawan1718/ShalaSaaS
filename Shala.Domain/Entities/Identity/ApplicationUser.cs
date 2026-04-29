using Microsoft.AspNetCore.Identity;
using Shala.Domain.Entities.Organization;
using Shala.Domain.Entities.Platform;


namespace Shala.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public int? TenantId { get; set; }
        public SchoolTenant? Tenant { get; set; }

        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<UserBranchAccess> BranchAccesses { get; set; } = new List<UserBranchAccess>();
    }
}