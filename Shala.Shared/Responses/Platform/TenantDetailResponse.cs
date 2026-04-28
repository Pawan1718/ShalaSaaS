namespace Shala.Shared.Responses.Platform;

public class TenantDetailResponse
{
    public int Id { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string SchoolCode { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; } = string.Empty;

    public string BusinessCategory { get; set; } = string.Empty;
    public string SubscriptionPlan { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}