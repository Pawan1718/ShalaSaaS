using Shala.Domain.Common;
using Shala.Domain.Enums;

namespace Shala.Domain.Entities.Students;

public class Guardian : AuditableEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public int StudentId { get; set; }

    public string Name { get; set; } = string.Empty;
    public RelationType RelationType { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Occupation { get; set; }
    public string? Address { get; set; }
    public bool IsPrimary { get; set; }

    public Student Student { get; set; } = default!;
}