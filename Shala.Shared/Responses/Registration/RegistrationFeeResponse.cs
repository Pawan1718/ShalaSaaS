namespace Shala.Shared.Responses.Registration
{
    public class RegistrationFeeResponse
    {
        public int RegistrationId { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;

        public int ChargeId { get; set; }
        public int? ProspectusChargeId { get; set; }

        public int ReceiptId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; }
        public DateTime ReceiptDate { get; set; }

        public decimal RegistrationAmount { get; set; }
        public decimal ProspectusAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public bool CanPrintReceipt { get; set; }
        public bool CanDownloadReceipt { get; set; }
        public bool AutoPrintAfterSave { get; set; }
    }
}