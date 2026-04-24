namespace Shala.Shared.Responses.Supplies;

public class SupplyItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal SalePrice { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public bool IsLowStock => CurrentStock <= MinimumStock;
    public bool IsActive { get; set; }
}