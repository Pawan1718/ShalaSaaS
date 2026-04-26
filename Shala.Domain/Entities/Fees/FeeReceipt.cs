using Shala.Domain.Common;
using Shala.Domain.Entities.Students;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shala.Domain.Entities.Fees;

public class FeeReceipt : BaseEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public int StudentId { get; set; }
    public int? StudentAdmissionId { get; set; }

    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

    public int PaymentMode { get; set; }
    public string? TransactionReference { get; set; }
    public string? Remarks { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsCancelled { get; set; } = false;
    public DateTime? CancelledOnUtc { get; set; }
    public string? CancelReason { get; set; }

    public virtual ICollection<FeeReceiptAllocation> Allocations { get; set; } = new List<FeeReceiptAllocation>();

    [ForeignKey(nameof(StudentId))]
    public virtual Student? Student { get; set; }

    [ForeignKey(nameof(StudentAdmissionId))]
    public virtual StudentAdmission? StudentAdmission { get; set; }
}