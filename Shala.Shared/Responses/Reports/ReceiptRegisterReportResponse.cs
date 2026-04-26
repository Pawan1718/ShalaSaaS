namespace Shala.Shared.Responses.Reports;

public sealed class ReceiptRegisterReportResponse
{
    public int ReceiptId { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }

    public int StudentId { get; set; }
    public int? AdmissionId { get; set; }
    public string AdmissionNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;

    public string PaymentMode { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }

    public bool IsCancelled { get; set; }
    public DateTime? CancelledOnUtc { get; set; }
    public string? CancelReason { get; set; }
}
