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
            return await AccessScopeValidator
                .ValidateBranchAccessAsync(branchId, cancellationToken);
        }

        protected async Task<List<int>> GetSafeBranchIdsAsync(
            CancellationToken cancellationToken = default)
        {
            return await AccessScopeValidator
                .GetAllowedBranchIdsAsync(cancellationToken);
        }
    }
}