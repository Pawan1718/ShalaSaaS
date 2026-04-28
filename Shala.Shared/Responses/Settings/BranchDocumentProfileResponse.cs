namespace Shala.Shared.Responses.Settings
{
    public class BranchDocumentProfileResponse
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public string? DisplayName { get; set; }
        public string? LogoUrl { get; set; }
        public string? AddressLine { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? PrimaryColorHex { get; set; }

        public string? ReceiptTitle { get; set; }
        public string? ReceiptFooterNote { get; set; }
        public string? SignatureLabel { get; set; }

        public bool ShowLogo { get; set; }
        public bool ShowAddress { get; set; }
        public bool ShowContactInfo { get; set; }
        public bool ShowStudentDetails { get; set; }
        public bool ShowFeeBreakup { get; set; }
        public bool ShowAmountInWords { get; set; }
        public bool ShowSignature { get; set; }

        public bool AllowPrintReceipt { get; set; }
        public bool AllowDownloadReceipt { get; set; }
        public bool AutoPrintAfterSave { get; set; }

        public bool IsActive { get; set; }
    }
}