namespace Shala.Shared.Responses.Fees;

public class FeeReceiptResponse
{
    public int Id { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;

    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }

    public DateTime ReceiptDate { get; set; }
    public int PaymentMode { get; set; }
    public string? TransactionReference { get; set; }
    public string? Remarks { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsCancelled { get; set; }
    public DateTime? CancelledOnUtc { get; set; }
    public string? CancelReason { get; set; }

    public List<FeeReceiptAllocationResponse> Allocations { get; set; } = new();
}