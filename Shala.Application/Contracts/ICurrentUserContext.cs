namespace Shala.Application.Contracts;

public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }

    string? UserId { get; }
    string? Email { get; }
    string? FullName { get; }
    string? Role { get; }

    int? TenantId { get; }
    int? BranchId { get; }

    int GetRequiredTenantId();
    int GetRequiredBranchId();
}