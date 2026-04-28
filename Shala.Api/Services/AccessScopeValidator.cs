using Microsoft.EntityFrameworkCore;
using Shala.Application.Contracts;
using Shala.Infrastructure.Data;

namespace Shala.Api.Services;

public sealed class AccessScopeValidator : IAccessScopeValidator
{
    private readonly ICurrentUserContext _currentUser;
    private readonly AppDbContext _db;

    public AccessScopeValidator(
        ICurrentUserContext currentUser,
        AppDbContext db)
    {
        _currentUser = currentUser;
        _db = db;
    }

    public void EnsureTenantAccess(int requestedTenantId)
    {
        if (requestedTenantId <= 0)
            throw new UnauthorizedAccessException("Invalid tenant.");

        if (_currentUser.IsPlatformAdmin)
            return;

        var claimTenantId = _currentUser.GetRequiredTenantId();

        if (claimTenantId != requestedTenantId)
            throw new UnauthorizedAccessException("You are not allowed to access this tenant.");
    }

    public void EnsureBranchAccess(int requestedBranchId)
    {
        if (requestedBranchId <= 0)
            throw new UnauthorizedAccessException("Invalid branch.");

        if (_currentUser.IsPlatformAdmin)
            return;

        var claimBranchId = _currentUser.GetRequiredBranchId();

        if (claimBranchId != requestedBranchId)
            throw new UnauthorizedAccessException("You are not allowed to access this branch.");
    }

    public void EnsureTenantAndBranchAccess(int requestedTenantId, int requestedBranchId)
    {
        EnsureTenantAccess(requestedTenantId);
        EnsureBranchAccess(requestedBranchId);
    }

    public async Task<int> ValidateBranchAccessAsync(
        int? requestedBranchId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUser.GetRequiredTenantId();

        var branchId = requestedBranchId ?? _currentUser.BranchId;

        if (!branchId.HasValue || branchId.Value <= 0)
            throw new UnauthorizedAccessException("BranchId is required.");

        var branchExistsInTenant = await _db.Branches
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == branchId.Value &&
                x.TenantId == tenantId &&
                x.IsActive,
                cancellationToken);

        if (!branchExistsInTenant)
            throw new UnauthorizedAccessException("Invalid branch for this tenant.");

        if (_currentUser.IsPlatformAdmin)
            return branchId.Value;

        var userId = _currentUser.GetRequiredUserId();

        var hasAccess = await _db.UserBranchAccesses
            .AsNoTracking()
            .AnyAsync(x =>
                x.TenantId == tenantId &&
                x.UserId == userId &&
                x.IsActive &&
                (
                    x.HasAllBranchesAccess ||
                    x.BranchId == branchId.Value
                ),
                cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("You do not have access to this branch.");

        return branchId.Value;
    }

    public async Task<List<int>> GetAllowedBranchIdsAsync(
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUser.GetRequiredTenantId();

        if (_currentUser.IsPlatformAdmin)
        {
            return await _db.Branches
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        var userId = _currentUser.GetRequiredUserId();

        var hasAllBranchesAccess = await HasAllBranchesAccessAsync(cancellationToken);

        if (hasAllBranchesAccess)
        {
            return await _db.Branches
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        return await _db.UserBranchAccesses
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.UserId == userId &&
                x.IsActive &&
                x.BranchId.HasValue)
            .Select(x => x.BranchId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAllBranchesAccessAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsPlatformAdmin)
            return true;

        var tenantId = _currentUser.GetRequiredTenantId();
        var userId = _currentUser.GetRequiredUserId();

        return await _db.UserBranchAccesses
            .AsNoTracking()
            .AnyAsync(x =>
                x.TenantId == tenantId &&
                x.UserId == userId &&
                x.IsActive &&
                x.HasAllBranchesAccess,
                cancellationToken);
    }
}