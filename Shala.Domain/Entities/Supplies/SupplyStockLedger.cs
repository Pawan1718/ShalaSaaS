using Shala.Domain.Common;
using Shala.Domain.Enums;

namespace Shala.Domain.Entities.Supplies;

public class SupplyStockLedger : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public int SupplyItemId { get; set; }
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    public SupplyMovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal BalanceAfter { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public string? Remarks { get; set; }

    public SupplyItem SupplyItem { get; set; } = default!;
}