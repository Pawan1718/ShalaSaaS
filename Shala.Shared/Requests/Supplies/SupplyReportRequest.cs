namespace Shala.Shared.Requests.Supplies;

public class SupplyReportRequest
{
    public int? AcademicYearId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? StudentId { get; set; }
    public int? SupplyItemId { get; set; }
}