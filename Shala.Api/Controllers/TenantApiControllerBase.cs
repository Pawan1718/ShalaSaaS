using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;

namespace Shala.Api.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class TenantApiControllerBase : ControllerBase
    {
        protected readonly ICurrentUserContext CurrentUser;
        protected readonly IAccessScopeValidator AccessScopeValidator;

        protected TenantApiControllerBase(
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
        {
            CurrentUser = currentUser;
            AccessScopeValidator = accessScopeValidator;
        }

        protected int TenantId => CurrentUser.GetRequiredTenantId();
        protected int BranchId => CurrentUser.GetRequiredBranchId();
        protected string Actor => CurrentUser.Email ?? CurrentUser.UserId ?? "system";
    }
}