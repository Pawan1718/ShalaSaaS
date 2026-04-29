namespace Shala.Shared.Requests.Tenant;

public sealed class TenantDashboardRequest
{
    public int? BranchId { get; set; }
    public bool IsAllBranches { get; set; }
}