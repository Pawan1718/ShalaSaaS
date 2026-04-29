using Shala.Domain.Common;

namespace Shala.Domain.Entities.Fees;

public class FeeReceiptAllocation : AuditableEntity
{
    public int FeeReceiptId { get; set; }
    public int StudentChargeId { get; set; }

    public decimal AllocatedAmount { get; set; }

    public FeeReceipt FeeReceipt { get; set; } = default!;
    public StudentCharge StudentCharge { get; set; } = default!;
}