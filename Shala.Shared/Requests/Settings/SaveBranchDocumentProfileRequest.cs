using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Settings
{
    public class SaveBranchDocumentProfileRequest
    {
        [MaxLength(200)]
        public string? DisplayName { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        [MaxLength(500)]
        public string? AddressLine { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? PrimaryColorHex { get; set; }

        [MaxLength(150)]
        public string? ReceiptTitle { get; set; }

        [MaxLength(500)]
        public string? ReceiptFooterNote { get; set; }

        [MaxLength(100)]
        public string? SignatureLabel { get; set; }

        public bool ShowLogo { get; set; } = true;
        public bool ShowAddress { get; set; } = true;
        public bool ShowContactInfo { get; set; } = true;
        public bool ShowStudentDetails { get; set; } = true;
        public bool ShowFeeBreakup { get; set; } = true;
        public bool ShowAmountInWords { get; set; } = true;
        public bool ShowSignature { get; set; } = true;

        public bool AllowPrintReceipt { get; set; } = true;
        public bool AllowDownloadReceipt { get; set; } = true;
        public bool AutoPrintAfterSave { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}