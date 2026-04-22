using Shala.Application.Contracts;

namespace Shala.Api.Services;

public sealed class AccessScopeValidator : IAccessScopeValidator
{
    private readonly ICurrentUserContext _currentUser;

    public AccessScopeValidator(ICurrentUserContext currentUser)
    {
        _currentUser = currentUser;
    }

    public void EnsureTenantAccess(int requestedTenantId)
    {
        if (requestedTenantId <= 0)
            throw new UnauthorizedAccessException("Invalid tenant.");

        var claimTenantId = _currentUser.GetRequiredTenantId();

        if (claimTenantId != requestedTenantId)
            throw new UnauthorizedAccessException("You are not allowed to access this tenant.");
    }

    public void EnsureBranchAccess(int requestedBranchId)
    {
        if (requestedBranchId <= 0)
            throw new UnauthorizedAccessException("Invalid branch.");

        var claimBranchId = _currentUser.GetRequiredBranchId();

        if (claimBranchId != requestedBranchId)
            throw new UnauthorizedAccessException("You are not allowed to access this branch.");
    }

    public void EnsureTenantAndBranchAccess(int requestedTenantId, int requestedBranchId)
    {
        EnsureTenantAccess(requestedTenantId);
        EnsureBranchAccess(requestedBranchId);
    }
}