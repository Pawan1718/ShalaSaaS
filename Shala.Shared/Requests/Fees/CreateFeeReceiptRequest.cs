namespace Shala.Shared.Requests.Fees;

public class CreateFeeReceiptRequest
{
    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }

    public int PaymentMode { get; set; }
    public string? TransactionReference { get; set; }
    public string? Remarks { get; set; }

    public List<FeeReceiptAllocationRequest> Allocations { get; set; } = new();
}