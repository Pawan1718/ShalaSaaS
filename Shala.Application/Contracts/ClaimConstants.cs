namespace Shala.Application.Contracts;

public static class ClaimConstants
{
    public const string UserId = "userId";
    public const string Email = "email";
    public const string FullName = "fullName";
    public const string TenantId = "tenantId";
    public const string BranchId = "branchId";
    public const string Role = "role";

    // Temporary backward compatibility
    public const string LegacyFullName = "FullName";
    public const string LegacyTenantId = "TenantId";
    public const string LegacyBranchId = "BranchId";
}