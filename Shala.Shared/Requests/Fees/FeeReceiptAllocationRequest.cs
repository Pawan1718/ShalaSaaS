namespace Shala.Shared.Requests.Fees;

public class FeeReceiptAllocationRequest
{
    public int StudentChargeId { get; set; }
    public decimal AllocatedAmount { get; set; }
}