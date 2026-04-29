namespace Shala.Web.Models.Fees;

public class FeeSummaryVm
{
    public decimal TotalAmount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalBalance { get; set; }

    public int TotalInstallments { get; set; }
    public int PaidInstallments { get; set; }
    public int PartialInstallments { get; set; }
    public int PendingInstallments { get; set; }
}