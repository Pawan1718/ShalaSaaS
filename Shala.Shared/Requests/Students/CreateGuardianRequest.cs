using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Students;

public class CreateGuardianRequest
{

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 4)]
    public int RelationType { get; set; }

    [Required]
    [Phone]
    [MaxLength(20)]
    public string Mobile { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(150)]
    public string? Occupation { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsPrimary { get; set; }
}