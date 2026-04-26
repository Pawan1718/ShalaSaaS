using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Students;

public class CreateAcademicYearRequest
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; } = true;
}