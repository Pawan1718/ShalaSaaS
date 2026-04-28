namespace Shala.Domain.Entities.Platform;

public class TenantDomain
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public string Domain { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }

    public SchoolTenant Tenant { get; set; } = default!;
}