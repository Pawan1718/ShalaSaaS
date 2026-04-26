namespace Shala.Shared.Responses.Tenant;

public class TenantBranchOptionDto
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public bool IsMainBranch { get; set; }
    public bool IsDefault { get; set; }
}