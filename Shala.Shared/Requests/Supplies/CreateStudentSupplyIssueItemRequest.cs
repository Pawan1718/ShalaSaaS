namespace Shala.Shared.Requests.Supplies;

public class CreateStudentSupplyIssueItemRequest
{
    public int SupplyItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
}