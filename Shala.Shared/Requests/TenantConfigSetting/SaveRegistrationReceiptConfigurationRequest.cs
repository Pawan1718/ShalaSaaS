using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.TenantConfigSetting
{
    public class SaveRegistrationReceiptConfigurationRequest
    {
        public bool AllowPrintReceipt { get; set; } = true;

        public bool AllowDownloadReceipt { get; set; } = true;

        public bool AutoPrintAfterSave { get; set; } = false;

        [MaxLength(150, ErrorMessage = "Receipt title cannot exceed 150 characters.")]
        public string? ReceiptTitle { get; set; }

        [MaxLength(500, ErrorMessage = "Receipt footer note cannot exceed 500 characters.")]
        public string? ReceiptFooterNote { get; set; }

        public bool ShowStudentDetailsInReceipt { get; set; } = true;

        public bool ShowFeeHeadInReceipt { get; set; } = true;

        public bool ShowAmountInWords { get; set; } = true;

        public bool IsActive { get; set; } = true;
    }
}