namespace Shala.Shared.Requests.Fees;

public class CreateFeeHeadRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public bool IsRegistrationFee { get; set; } = false;
    public bool IsActive { get; set; } = true;
}