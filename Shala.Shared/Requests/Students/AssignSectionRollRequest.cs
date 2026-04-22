using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Students;

public class AssignSectionRollRequest
{
    [Range(1, int.MaxValue)]
    public int StudentAdmissionId { get; set; }

    [Range(1, int.MaxValue)]
    public int AcademicYearId { get; set; }

    [Range(1, int.MaxValue)]
    public int ClassId { get; set; }

    [Range(1, int.MaxValue)]
    public int? SectionId { get; set; }

    public bool AutoGenerateRollNo { get; set; } = true;

    [MaxLength(20)]
    public string? RollNo { get; set; }
}