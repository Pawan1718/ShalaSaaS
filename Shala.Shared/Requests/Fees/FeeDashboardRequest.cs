using Shala.Shared.Common;

namespace Shala.Shared.Requests.Fees;

public sealed class FeeDashboardRequest : PagedRequest
{
    public string? WorkflowStatus { get; set; }

    public int? AcademicYearId { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
}