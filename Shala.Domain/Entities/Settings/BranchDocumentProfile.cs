using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shala.Domain.Common;

namespace Shala.Domain.Entities.Settings
{
    [Table("BranchDocumentProfiles")]
    public class BranchDocumentProfile : AuditableEntity, ITenantEntity, IBranchEntity
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BranchId { get; set; }

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

        [Required]
        public bool ShowLogo { get; set; } = true;

        [Required]
        public bool ShowAddress { get; set; } = true;

        [Required]
        public bool ShowContactInfo { get; set; } = true;

        [Required]
        public bool ShowStudentDetails { get; set; } = true;

        [Required]
        public bool ShowFeeBreakup { get; set; } = true;

        [Required]
        public bool ShowAmountInWords { get; set; } = true;

        [Required]
        public bool ShowSignature { get; set; } = true;

        [Required]
        public bool AllowPrintReceipt { get; set; } = true;

        [Required]
        public bool AllowDownloadReceipt { get; set; } = true;

        [Required]
        public bool AutoPrintAfterSave { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;
    }
}