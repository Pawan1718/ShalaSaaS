using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Academics;

public class CreateStudentAdmissionRequest
{
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Range(1, int.MaxValue)]
    public int AcademicYearId { get; set; }

    [Range(1, int.MaxValue)]
    public int ClassId { get; set; }

    [Range(1, int.MaxValue)]
    public int? SectionId { get; set; }

    [MaxLength(20)]
    public string? RollNo { get; set; }

    [Required]
    public DateTime AdmissionDate { get; set; }
}