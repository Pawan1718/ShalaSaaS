using Shala.Domain.Enums;

namespace Shala.Shared.Requests.Supplies;

public class CollectSupplyDueRequest
{
    public decimal Amount { get; set; }
    public SupplyPaymentMode PaymentMode { get; set; } = SupplyPaymentMode.Cash;
    public string? ReferenceNo { get; set; }
    public string? Remarks { get; set; }
}