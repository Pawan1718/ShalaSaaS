using Shala.Domain.Common;
using Shala.Domain.Enums;

namespace Shala.Domain.Entities.Supplies;

public class StudentSupplyPayment : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }
    public int AcademicYearId { get; set; }

    public int StudentSupplyIssueId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public SupplyPaymentMode PaymentMode { get; set; } = SupplyPaymentMode.Cash;
    public string? ReferenceNo { get; set; }
    public string? Remarks { get; set; }

    public StudentSupplyIssue StudentSupplyIssue { get; set; } = default!;
}