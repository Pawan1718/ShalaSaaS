namespace Shala.Application.Contracts;

public interface IAccessScopeValidator
{
    void EnsureTenantAccess(int requestedTenantId);
    void EnsureBranchAccess(int requestedBranchId);
    void EnsureTenantAndBranchAccess(int requestedTenantId, int requestedBranchId);

    Task<int> ValidateBranchAccessAsync(
        int? requestedBranchId,
        CancellationToken cancellationToken = default);

    Task<List<int>> GetAllowedBranchIdsAsync(
        CancellationToken cancellationToken = default);

    Task<bool> HasAllBranchesAccessAsync(
        CancellationToken cancellationToken = default);
}