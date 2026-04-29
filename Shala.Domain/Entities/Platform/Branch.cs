using Shala.Domain.Entities.Identity;
using Shala.Domain.Entities.Organization;


namespace Shala.Domain.Entities.Platform;

public class Branch
{
    public int Id { get; set; }

    public int TenantId { get; set; }
    public SchoolTenant Tenant { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Pincode { get; set; }

    public string? PrincipalName { get; set; }

    public bool IsMainBranch { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    public ICollection<UserBranchAccess> UserBranchAccesses { get; set; } = new List<UserBranchAccess>();

}