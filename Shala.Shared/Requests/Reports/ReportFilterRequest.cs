using Shala.Shared.Common;

namespace Shala.Shared.Requests.Reports;

public sealed class ReportFilterRequest : PagedRequest
{
    public int? AcademicYearId { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? StudentId { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string? Status { get; set; }
    public string? PaymentMode { get; set; }
}
