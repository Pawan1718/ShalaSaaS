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
            return BadRequest(result);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateTenantUserRequest request)
    {
        var tenantId = _currentUser.GetRequiredTenantId();

        var result = await _userService.CreateUserAsync(tenantId, request);

        if (!result.Success)
            return BadRequest(result.Data);

        return Ok(result.Data);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(
        string userId,
        [FromBody] UpdateTenantUserRequest request)
    {
        var tenantId = _currentUser.GetRequiredTenantId();

        var result = await _userService.UpdateUserAsync(tenantId, userId, request);

        if (!result.Success)
            return BadRequest(result.Data);

        return Ok(result.Data);
    }

    [HttpPatch("{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        string userId,
        [FromBody] UpdateTenantUserStatusRequest request)
    {
        var tenantId = _currentUser.GetRequiredTenantId();

        var result = await _userService.UpdateUserStatusAsync(tenantId, userId, request);

        if (!result.Success)
            return BadRequest(result.Data);

        return Ok(result.Data);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var tenantId = _currentUser.GetRequiredTenantId();

        var result = await _userService.DeleteUserAsync(tenantId, userId);

        if (!result.Success)
            return BadRequest(result.Data);

        return Ok(result.Data);
    }
}