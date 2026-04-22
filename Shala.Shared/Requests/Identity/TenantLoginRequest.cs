using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Identity;

public class TenantLoginRequest
{
    [Required]
    [MaxLength(256)]
    public string UserIdOrEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? BranchCode { get; set; }
}