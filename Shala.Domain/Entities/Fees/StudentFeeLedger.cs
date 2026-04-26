using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shala.Domain.Common;

namespace Shala.Domain.Entities.Fees;

[Table("StudentFeeLedgers")]
public class StudentFeeLedger : AuditableEntity, ITenantEntity, IBranchEntity
{
    [Required]
    public int TenantId { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int StudentAdmissionId { get; set; }

    public int? StudentChargeId { get; set; }
    public int? FeeReceiptId { get; set; }
    public int? FeeHeadId { get; set; }

    [Required]
    [MaxLength(50)]
    public string EntryType { get; set; } = string.Empty;
    // Charge, Discount, Fine, Receipt, ReceiptCancel, Adjustment

    [Required]
    public DateTime EntryDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal DebitAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RunningBalance { get; set; }

    [MaxLength(100)]
    public string? ReferenceNo { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}