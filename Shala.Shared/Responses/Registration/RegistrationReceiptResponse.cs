namespace Shala.Shared.Responses.Registration
{
    public class RegistrationReceiptResponse
    {
        public int ReceiptId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }

        public int RegistrationId { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }

        public string? ReceiptTitle { get; set; }
        public string? ReceiptFooterNote { get; set; }

        public bool ShowStudentDetailsInReceipt { get; set; }
        public bool ShowFeeHeadInReceipt { get; set; }
        public bool ShowAmountInWords { get; set; }
        public bool ShowProspectusInReceipt { get; set; }

        public bool CanPrintReceipt { get; set; }
        public bool CanDownloadReceipt { get; set; }
        public bool AutoPrintAfterSave { get; set; }

        public decimal TotalAmount { get; set; }

        public List<RegistrationReceiptLineResponse> Lines { get; set; } = new();
    }
}