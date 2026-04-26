using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Identity;

public class PlatformLoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}