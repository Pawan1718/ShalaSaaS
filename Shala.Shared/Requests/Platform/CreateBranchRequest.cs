using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Platform;

public class CreateBranchRequest
{
    [Required]
    public int TenantId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? AddressLine1 { get; set; }

    [MaxLength(200)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? Pincode { get; set; }

    [MaxLength(150)]
    public string? PrincipalName { get; set; }

    public bool IsMainBranch { get; set; }
    public bool IsActive { get; set; } = true;
}