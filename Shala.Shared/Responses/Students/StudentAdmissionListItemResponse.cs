namespace Shala.Shared.Responses.Students;

public class StudentAdmissionListItemResponse
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public string? RollNo { get; set; }
    public DateTime AdmissionDate { get; set; }
    public string Status { get; set; } = string.Empty;
}