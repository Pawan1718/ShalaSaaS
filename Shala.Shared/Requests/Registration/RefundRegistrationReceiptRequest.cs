namespace Shala.Shared.Requests.Registration;

public sealed class RefundRegistrationReceiptRequest
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}