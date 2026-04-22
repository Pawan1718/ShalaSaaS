namespace Shala.Application.Contracts;

public interface IAccessScopeValidator
{
    void EnsureTenantAccess(int requestedTenantId);
    void EnsureBranchAccess(int requestedBranchId);
    void EnsureTenantAndBranchAccess(int requestedTenantId, int requestedBranchId);
}