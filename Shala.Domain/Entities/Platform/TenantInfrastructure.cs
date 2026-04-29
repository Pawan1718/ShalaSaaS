namespace Shala.Domain.Entities.Platform;

public class TenantInfrastructure
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public string TenantCode { get; set; } = string.Empty;
    public string HostingMode { get; set; } = "Shared";
    public string? ServerName { get; set; }
    public string? DatabaseName { get; set; }
    public string? ConnectionString { get; set; }

    public bool IsActive { get; set; } = true;

    public SchoolTenant Tenant { get; set; } = default!;
}