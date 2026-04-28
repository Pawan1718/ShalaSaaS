using Shala.Domain.Enums;

namespace Shala.Shared.Requests.Supplies;

public class AddSupplyStockRequest
{
    public int SupplyItemId { get; set; }
    public decimal Quantity { get; set; }
    public SupplyMovementType MovementType { get; set; } = SupplyMovementType.In;
    public string? Remarks { get; set; }
}