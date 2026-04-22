using Shala.Domain.Common;
using Shala.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shala.Domain.Entities.Registration
{
    [Table("RegistrationFeeReceipts")]
    public class RegistrationFeeReceipt : AuditableEntity, ITenantEntity, IBranchEntity
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BranchId { get; set; }

        public int? RegistrationId { get; set; }

        [Required]
        [MaxLength(30)]
        public string ReceiptNo { get; set; } = string.Empty;

        [Required]
        public DateTime ReceiptDate { get; set; }

        [Required]
        public PaymentMode PaymentMode { get; set; }

        [MaxLength(100)]
        public string? TransactionReference { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999.99)]
        public decimal RegistrationAmount { get; set; }

        [Required]
        public bool IsProspectusIncluded { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999.99)]
        public decimal ProspectusAmount { get; set; }

        [MaxLength(100)]
        public string? ProspectusLabel { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999.99)]
        public decimal TotalAmount { get; set; }

        [Required]
        public bool IsRegistrationReceipt { get; set; } = true;
    }
}