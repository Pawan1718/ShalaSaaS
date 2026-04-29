using Shala.Domain.Enums;

namespace Shala.Shared.Responses.Supplies;

public class SupplyStockLedgerResponse
{
    public int Id { get; set; }
    public int SupplyItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public DateTime MovementDate { get; set; }
    public SupplyMovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal BalanceAfter { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public string? Remarks { get; set; }
}