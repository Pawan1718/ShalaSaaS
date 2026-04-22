namespace Shala.Shared.Responses.TenantConfigSetting
{
    public class RegistrationReceiptConfigurationResponse
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public bool AllowPrintReceipt { get; set; }
        public bool AllowDownloadReceipt { get; set; }
        public bool AutoPrintAfterSave { get; set; }

        public string? ReceiptTitle { get; set; }
        public string? ReceiptFooterNote { get; set; }

        public bool ShowStudentDetailsInReceipt { get; set; }
        public bool ShowFeeHeadInReceipt { get; set; }
        public bool ShowAmountInWords { get; set; }

        public bool IsActive { get; set; }
    }
}