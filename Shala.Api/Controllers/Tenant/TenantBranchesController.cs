using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shala.Api.Services;
using Shala.Infrastructure.Data;
using Shala.Shared.Responses.Tenant;

namespace Shala.Api.Controllers.Tenant;

[Route("api/tenant/branches")]
public class TenantBranchesController : TenantApiControllerBase
{
    private readonly AppDbContext _context;

    public TenantBranchesController(
        AppDbContext context,
        Shala.Application.Contracts.ICurrentUserContext currentUser,
        Shala.Application.Contracts.IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _context = context;
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<TenantBranchOptionDto>>> GetMyBranches(CancellationToken cancellationToken)
    {
        var tenantId = TenantId;
        var userId = CurrentUser.UserId;
        var role = CurrentUser.Role ?? string.Empty;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var isTenantAdmin =
            role.Equals("SchoolAdmin", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("TenantAdmin", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("TenantOwner", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Owner", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);

        if (isTenantAdmin)
        {
            var adminBranches = await _context.Branches
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
                    IsDefault = x.IsMainBranch
                })
                .ToListAsync(cancellationToken);

            return Ok(adminBranches);
        }

        var branches = await _context.UserBranchAccesses
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.IsActive &&
                x.Branch.IsActive &&
                x.Branch.TenantId == tenantId)
            .OrderByDescending(x => x.Branch.IsMainBranch)
            .ThenByDescending(x => x.IsDefault)
            .ThenBy(x => x.Branch.Name)
            .Select(x => new TenantBranchOptionDto
            {
                BranchId = x.Branch.Id,
                BranchName = x.Branch.Name,
                BranchCode = x.Branch.Code,
                IsMainBranch = x.Branch.IsMainBranch,
                IsDefault = x.IsDefault
            })
            .ToListAsync(cancellationToken);

        return Ok(branches);
    }
}