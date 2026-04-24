using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shala.Domain.Common;

namespace Shala.Domain.Entities.Registration;

[Table("RegistrationFeeReceiptAudits")]
public class RegistrationFeeReceiptAudit : AuditableEntity, ITenantEntity, IBranchEntity
{
    [Required]
    public int TenantId { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    public int ReceiptId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Reason { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Amount { get; set; }

    [MaxLength(100)]
    public string? PerformedBy { get; set; }

    public DateTime PerformedOn { get; set; } = DateTime.UtcNow;
}