namespace Shala.Shared.Responses.Reports;

public sealed class PendingDocumentReportResponse
{
    public int StudentId { get; set; }
    public int AdmissionId { get; set; }

    public string AdmissionNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;

    public string AcademicYearName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }

    public int RequiredDocumentCount { get; set; }
    public int ReceivedDocumentCount { get; set; }
    public int PendingDocumentCount { get; set; }
}
