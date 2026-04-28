namespace Shala.Shared.Responses.Supplies;

public class SupplyDashboardResponse
{
    public int TodayIssueCount { get; set; }
    public decimal TodayCollection { get; set; }
    public decimal PendingDue { get; set; }
    public int LowStockItemCount { get; set; }
}