using Shala.Domain.Common;

namespace Shala.Domain.Entities.Supplies;

public class StudentSupplyIssueItem : AuditableEntity
{
    public int StudentSupplyIssueId { get; set; }
    public int SupplyItemId { get; set; }

    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public StudentSupplyIssue StudentSupplyIssue { get; set; } = default!;
    public SupplyItem SupplyItem { get; set; } = default!;
}