using Shala.Shared.Common;

namespace Shala.Shared.Requests.Fees;

public sealed class FeeLedgerDashboardRequest : PagedRequest
{
    public int? StudentId { get; set; }
    public int? AdmissionId { get; set; }

    public string? EntryType { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}