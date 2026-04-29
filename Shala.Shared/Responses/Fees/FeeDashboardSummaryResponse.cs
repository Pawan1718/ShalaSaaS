namespace Shala.Shared.Responses.Fees;

public sealed class FeeDashboardSummaryResponse
{
    public int TotalStudents { get; set; }
    public int AssignedPlans { get; set; }
    public int CollectibleStudents { get; set; }
    public int FullyPaidStudents { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalBalance { get; set; }
}