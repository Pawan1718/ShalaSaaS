namespace Shala.Shared.Responses.Platform;

public class BranchResponse
{
    public int Id { get; set; }
    public int TenantId { get; set; }

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
    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}