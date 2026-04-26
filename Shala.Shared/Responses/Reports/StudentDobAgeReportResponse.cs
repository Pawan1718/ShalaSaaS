namespace Shala.Shared.Responses.Reports;

public sealed class StudentDobAgeReportResponse
{
    public int StudentId { get; set; }
    public string AdmissionNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;

    public string? ClassName { get; set; }
    public string? SectionName { get; set; }
    public string? RollNo { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public int? AgeInYears { get; set; }
    public string AgeText { get; set; } = "-";
}