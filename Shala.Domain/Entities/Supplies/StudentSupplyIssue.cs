using Shala.Domain.Common;
using Shala.Domain.Entities.Students;
using Shala.Domain.Enums;

namespace Shala.Domain.Entities.Supplies;

public class StudentSupplyIssue : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }
    public int AcademicYearId { get; set; }

    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }

    public string IssueNo { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public SupplyPaymentStatus PaymentStatus { get; set; } = SupplyPaymentStatus.Due;

    public string? Remarks { get; set; }
    public bool IsCancelled { get; set; }
    public string? CancelReason { get; set; }

    public Student Student { get; set; } = default!;
    public StudentAdmission StudentAdmission { get; set; } = default!;
    public ICollection<StudentSupplyIssueItem> Items { get; set; } = new List<StudentSupplyIssueItem>();
    public ICollection<StudentSupplyPayment> Payments { get; set; } = new List<StudentSupplyPayment>();
}