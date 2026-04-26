namespace Shala.Web.Models.Fees;

public class CollectFeeRowVm
{
    public int StudentChargeId { get; set; }
    public string ChargeLabel { get; set; } = string.Empty;
    public string? PeriodLabel { get; set; }
    public DateTime DueDate { get; set; }

    public decimal NetAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }

    public bool Selected { get; set; }
    public decimal PayNowAmount { get; set; }
}