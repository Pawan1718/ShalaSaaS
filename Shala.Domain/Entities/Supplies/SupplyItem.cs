using Shala.Domain.Common;

namespace Shala.Domain.Entities.Supplies;

public class SupplyItem : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal SalePrice { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StudentSupplyIssueItem> IssueItems { get; set; } = new List<StudentSupplyIssueItem>();
    public ICollection<SupplyStockLedger> StockLedgers { get; set; } = new List<SupplyStockLedger>();
}