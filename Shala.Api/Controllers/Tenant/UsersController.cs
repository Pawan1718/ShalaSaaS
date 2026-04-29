using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Identity;
using Shala.Shared.Requests.Tenant;

namespace Shala.Api.Controllers.Tenant;

[ApiController]
[Route("api/tenant/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserContext _currentUser;

    public UsersController(
        IUserService userService,
        ICurrentUserContext currentUser)
    {
        _userService = userService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var tenantId = _currentUser.GetRequiredTenantId();

        var result = await _userService.GetUsersAsync(tenantId);

        if (!result.Success)
            return BadRequest(result.Data);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateTenantUserRequest request)
    {
        var tenantId = _currentUser.GetRequiredTenantId();

        var actorBranchId = _currentUser.GetRequiredBranchId();
        var actorRole = _currentUser.Role;

        var result = await _userService.CreateUserAsync(tenantId, actorBranchId, actorRole, request);

        if (!result.Success)
            return BadRequest(result.Data);

        return Ok(result.Data);
    }
}