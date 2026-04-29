namespace Shala.Shared.Responses.Students;

public class StudentListItemResponse
{
    public int Id { get; set; }
    public string AdmissionNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Mobile { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CurrentClass { get; set; }
    public string? CurrentSection { get; set; }
    public string? CurrentAcademicYear { get; set; }
}