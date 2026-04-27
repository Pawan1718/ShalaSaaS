using Shala.Domain.Entities.Identity;
using Shala.Domain.Entities.Platform;

namespace Shala.Domain.Entities.Organization;

public class UserBranchAccess
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public bool HasAllBranchesAccess { get; set; }

    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}