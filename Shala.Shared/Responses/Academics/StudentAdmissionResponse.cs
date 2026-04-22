namespace Shala.Shared.Responses.Academics;

public class StudentAdmissionResponse
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string AdmissionNo { get; set; } = string.Empty;

    public string AcademicYear { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string? RollNo { get; set; }
    public DateTime AdmissionDate { get; set; }
    public string Status { get; set; } = string.Empty;
}