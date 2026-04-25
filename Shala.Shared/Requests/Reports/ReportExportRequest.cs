using Shala.Shared.Common;

namespace Shala.Shared.Requests.Reports;

public sealed class ReportExportRequest : PagedRequest
{
    public int? AcademicYearId { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? StudentId { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string? Status { get; set; }
    public string? PaymentMode { get; set; }

    public string ReportKey { get; set; } = string.Empty;
    public string ExportType { get; set; } = "excel";

    public ReportFilterRequest ToFilterRequest()
    {
        return new ReportFilterRequest
        {
            AcademicYearId = AcademicYearId,
            ClassId = ClassId,
            SectionId = SectionId,
            StudentId = StudentId,
            FromDate = FromDate,
            ToDate = ToDate,
            Status = Status,
            PaymentMode = PaymentMode,
            SearchText = SearchText,
            SortBy = SortBy,
            SortDescending = SortDescending,
            PageNumber = PageNumber,
            PageSize = PageSize
        };
    }
}