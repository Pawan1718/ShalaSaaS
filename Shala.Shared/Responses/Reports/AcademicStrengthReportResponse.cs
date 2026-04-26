namespace Shala.Shared.Responses.Reports;

public sealed class AcademicStrengthReportResponse
{
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;

    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;

    public int? SectionId { get; set; }
    public string? SectionName { get; set; }

    public int StudentCount { get; set; }
}
