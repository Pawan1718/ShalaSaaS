namespace Shala.Shared.Responses.Reports;

public sealed class AdmissionRegisterReportResponse
{
    public int AdmissionId { get; set; }
    public int StudentId { get; set; }

    public string AdmissionNo { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string AcademicYearName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public string? RollNo { get; set; }

    public string AdmissionStatus { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
}
