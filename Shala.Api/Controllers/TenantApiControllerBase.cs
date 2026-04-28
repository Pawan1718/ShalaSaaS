using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;

namespace Shala.Api.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class TenantApiControllerBase : ControllerBase
    {
        protected ICurrentUserContext CurrentUser { get; }
        protected IAccessScopeValidator AccessScopeValidator { get; }

        protected TenantApiControllerBase(
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
        {
            CurrentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            AccessScopeValidator = accessScopeValidator ?? throw new ArgumentNullException(nameof(accessScopeValidator));
        }

        protected int TenantId => CurrentUser.GetRequiredTenantId();

        protected int BranchId => CurrentUser.GetRequiredBranchId(); // legacy only

        protected string Actor => CurrentUser.Email ?? CurrentUser.UserId ?? "system";

        protected async Task<int> GetSafeBranchIdAsync(
            int? branchId,
            CancellationToken cancellationToken = default)
        {
            var headerBranchId = GetBranchIdFromRequestHeader();

            if (!branchId.HasValue && !headerBranchId.HasValue && IsAllBranchesRequest())
                throw new UnauthorizedAccessException("Please select a specific branch for this page.");

            var effectiveBranchId = branchId ?? headerBranchId;

            return await AccessScopeValidator
                .ValidateBranchAccessAsync(effectiveBranchId, cancellationToken);
        }

        protected int? GetBranchIdFromRequestHeader()
        {
            if (!Request.Headers.TryGetValue("X-Branch-Id", out var values))
                return null;

            var raw = values.FirstOrDefault();

            return int.TryParse(raw, out var branchId) && branchId > 0
                ? branchId
                : null;
        }

        protected bool IsAllBranchesRequest()
        {
            if (!Request.Headers.TryGetValue("X-All-Branches", out var values))
                return false;

            var raw = values.FirstOrDefault();

            return bool.TryParse(raw, out var isAllBranches) && isAllBranches;
        }

        protected async Task<List<int>> GetSafeBranchIdsAsync(
            CancellationToken cancellationToken = default)
        {
            return await AccessScopeValidator
                .GetAllowedBranchIdsAsync(cancellationToken);
        }
    }
}