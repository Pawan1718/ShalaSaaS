namespace Shala.Shared.Responses.Supplies;

public class StudentSupplyIssueItemResponse
{
    public int Id { get; set; }
    public int SupplyItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}