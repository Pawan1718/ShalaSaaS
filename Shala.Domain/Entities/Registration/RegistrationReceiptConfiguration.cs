using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shala.Domain.Common;

namespace Shala.Domain.Entities.Registration
{
    [Table("RegistrationReceiptConfigurations")]
    public class RegistrationReceiptConfiguration : AuditableEntity, ITenantEntity, IBranchEntity
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public bool AllowPrintReceipt { get; set; } = true;

        [Required]
        public bool AllowDownloadReceipt { get; set; } = true;

        [Required]
        public bool AutoPrintAfterSave { get; set; } = false;

        [MaxLength(150)]
        public string? ReceiptTitle { get; set; }

        [MaxLength(500)]
        public string? ReceiptFooterNote { get; set; }

        [Required]
        public bool ShowStudentDetailsInReceipt { get; set; } = true;

        [Required]
        public bool ShowFeeHeadInReceipt { get; set; } = true;

        [Required]
        public bool ShowAmountInWords { get; set; } = true;

        [Required]
        public bool IsActive { get; set; } = true;
    }
}