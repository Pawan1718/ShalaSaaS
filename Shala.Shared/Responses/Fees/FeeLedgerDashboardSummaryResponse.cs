namespace Shala.Shared.Responses.Fees;

public sealed class FeeLedgerDashboardSummaryResponse
{
    public int TotalEntries { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal ClosingBalance { get; set; }
}