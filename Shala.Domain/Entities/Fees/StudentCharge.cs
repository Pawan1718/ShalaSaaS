using Shala.Domain.Common;
using Shala.Domain.Entities.Students;

namespace Shala.Domain.Entities.Fees;

public class StudentCharge : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public int? StudentId { get; set; }
    public int? StudentAdmissionId { get; set; }
    public int? StudentFeeAssignmentId { get; set; }
    public int FeeHeadId { get; set; }

    public string ChargeLabel { get; set; } = string.Empty;   // Apr Tuition, Annual Fee
    public string? PeriodLabel { get; set; }                  // Apr-2026, 2026-27

    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FineAmount { get; set; }
    public decimal PaidAmount { get; set; }

    public bool IsRegistrationCharge { get; set; } = false;
    public bool IsSettled { get; set; }
    public bool IsCancelled { get; set; }

    public Student Student { get; set; } = default!;
    public StudentAdmission StudentAdmission { get; set; } = default!;
    public StudentFeeAssignment StudentFeeAssignment { get; set; } = default!;
    public FeeHead FeeHead { get; set; } = default!;

    public ICollection<FeeReceiptAllocation> Allocations { get; set; } = new List<FeeReceiptAllocation>();

    public decimal NetAmount => Amount - DiscountAmount + FineAmount;
    public decimal BalanceAmount => NetAmount - PaidAmount;
}