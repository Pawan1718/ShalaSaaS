namespace Shala.Shared.Responses.Students;

public class SectionRollAssignmentDetailResponse
{
    public int StudentAdmissionId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;

    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;

    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;

    public int? CurrentSectionId { get; set; }
    public string? CurrentSectionName { get; set; }
    public string? CurrentRollNo { get; set; }

    public DateTime AdmissionDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public bool AutoGenerateEnabled { get; set; }
    public bool AllowManualOverride { get; set; }
}