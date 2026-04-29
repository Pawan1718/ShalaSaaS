using Shala.Shared.Responses.Supplies;

namespace Shala.Web.Models.Supplies;

public sealed class SupplyIssueRowVm
{
    public int SupplyItemId { get; set; }
    public SupplyItemResponse? Item { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => Quantity * UnitPrice;
}