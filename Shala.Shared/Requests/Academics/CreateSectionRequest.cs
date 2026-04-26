using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Students;

public class CreateSectionRequest
{
    [Range(1, int.MaxValue)]
    public int AcademicClassId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 100000)]
    public int? Capacity { get; set; }

    public bool IsActive { get; set; } = true;
}