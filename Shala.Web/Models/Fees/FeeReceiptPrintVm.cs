namespace Shala.Web.Models.Fees;

public sealed class FeeReceiptPrintVm
{
    public int ReceiptId { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }

    public string ReceiptTitle { get; set; } = "Fee Receipt";
    public string? ReceiptFooterNote { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public string? RollNo { get; set; }
    public string? GuardianName { get; set; }
    public string? Phone { get; set; }

    public string PaymentMode { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? Remarks { get; set; }

    public bool ShowStudentDetailsInReceipt { get; set; } = true;
    public bool ShowFeeHeadInReceipt { get; set; } = true;
    public bool ShowAmountInWords { get; set; } = true;
    public bool AllowPrintReceipt { get; set; } = true;
    public bool AllowDownloadReceipt { get; set; } = true;

    public decimal TotalAmount { get; set; }
    public string AmountInWords { get; set; } = string.Empty;

    public List<FeeReceiptPrintLineVm> Lines { get; set; } = new();
}

public sealed class FeeReceiptPrintLineVm
{
    public string Label { get; set; } = string.Empty;
    public string? FeeHeadName { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal Amount { get; set; }
}