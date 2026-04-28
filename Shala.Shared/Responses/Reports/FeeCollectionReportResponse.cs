namespace Shala.Shared.Responses.Reports;

public sealed class FeeCollectionReportResponse
{
    public int ReceiptId { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }

    public int StudentId { get; set; }
    public int? AdmissionId { get; set; }
    public string AdmissionNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;

    public string? AcademicYearName { get; set; }
    public string? ClassName { get; set; }
    public string? SectionName { get; set; }

    public string PaymentMode { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public string? TransactionReference { get; set; }
    public string? Remarks { get; set; }
}
