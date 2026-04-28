using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shala.Application.Contracts;
using Shala.Infrastructure.Data;
using Shala.Shared.Responses.Tenant;

namespace Shala.Api.Controllers.Tenant;

[Route("api/tenant/branches")]
public class TenantBranchesController : TenantApiControllerBase
{
    private readonly AppDbContext _context;

    public TenantBranchesController(
        AppDbContext context,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _context = context;
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<TenantBranchOptionDto>>> GetMyBranches(
        CancellationToken cancellationToken)
    {
        var tenantId = TenantId;
        var userId = CurrentUser.GetRequiredUserId();

        var hasAllBranchesAccess = await AccessScopeValidator
            .HasAllBranchesAccessAsync(cancellationToken);

        if (CurrentUser.IsPlatformAdmin || hasAllBranchesAccess)
        {
            var allBranches = await _context.Branches
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.IsActive)
                .OrderByDescending(x => x.IsMainBranch)
                .ThenBy(x => x.Name)
                .Select(x => new TenantBranchOptionDto
                {
                    BranchId = x.Id,
                    BranchName = x.Name,
                    BranchCode = x.Code,
                    IsMainBranch = x.IsMainBranch,
                    IsDefault = x.Id == CurrentUser.BranchId || x.IsMainBranch
                })
                .ToListAsync(cancellationToken);

            return Ok(allBranches);
        }

        var branches = await _context.UserBranchAccesses
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.UserId == userId &&
                x.IsActive &&
                x.BranchId.HasValue &&
                x.Branch != null &&
                x.Branch.IsActive &&
                x.Branch.TenantId == tenantId)
            .OrderByDescending(x => x.Branch!.IsMainBranch)
            .ThenByDescending(x => x.IsDefault)
            .ThenBy(x => x.Branch!.Name)
            .Select(x => new TenantBranchOptionDto
            {
                BranchId = x.BranchId!.Value,
                BranchName = x.Branch!.Name,
                BranchCode = x.Branch.Code,
                IsMainBranch = x.Branch.IsMainBranch,
                IsDefault = x.IsDefault || x.BranchId == CurrentUser.BranchId
            })
            .Distinct()
            .ToListAsync(cancellationToken);

        return Ok(branches);
    }
}