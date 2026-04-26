using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Academics;

public class CreateAcademicClassRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    [Range(1, int.MaxValue)]
    public int Sequence { get; set; }

    public bool IsActive { get; set; } = true;
}