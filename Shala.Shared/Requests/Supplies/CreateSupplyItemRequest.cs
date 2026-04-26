namespace Shala.Shared.Requests.Supplies;

public class CreateSupplyItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal SalePrice { get; set; }
    public decimal OpeningStock { get; set; }
    public decimal MinimumStock { get; set; }
    public bool IsActive { get; set; } = true;
}