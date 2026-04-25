namespace Shala.Shared.Responses.Reports;

public sealed class StudentMasterReportResponse
{
    public int StudentId { get; set; }
    public int AdmissionId { get; set; }

    public string AdmissionNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }

    public string AcademicYearName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public string? RollNo { get; set; }

    public string? GuardianName { get; set; }
    public string? GuardianMobile { get; set; }


    public string StudentStatus { get; set; } = string.Empty;
    public string AdmissionStatus { get; set; } = string.Empty;
}
