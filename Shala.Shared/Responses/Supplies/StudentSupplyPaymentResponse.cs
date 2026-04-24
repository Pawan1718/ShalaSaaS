using Shala.Domain.Enums;

namespace Shala.Shared.Responses.Supplies;

public class StudentSupplyPaymentResponse
{
    public int Id { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public SupplyPaymentMode PaymentMode { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Remarks { get; set; }
}