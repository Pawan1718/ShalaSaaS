namespace Shala.Shared.Responses.Fees;

public class FeeReceiptAllocationResponse
{
    public int StudentChargeId { get; set; }
    public int FeeHeadId { get; set; }

    public string? ChargeLabel { get; set; }
    public string? FeeHeadName { get; set; }
    public string? PeriodLabel { get; set; }
    public DateTime? DueDate { get; set; }

    public decimal AllocatedAmount { get; set; }
}