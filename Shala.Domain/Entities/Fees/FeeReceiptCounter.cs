using Shala.Domain.Common;

namespace Shala.Domain.Entities.Fees;

public sealed class FeeReceiptCounter : BaseEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }
    public int Year { get; set; }
    public int LastNumber { get; set; }
}