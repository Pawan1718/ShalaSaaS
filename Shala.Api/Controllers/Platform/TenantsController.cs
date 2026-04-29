using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shala.Application.Features.Platform;
using Shala.Shared.Common;
using Shala.Shared.Requests.Platform;

namespace Shala.Api.Controllers.Platform;

[Route("api/platform/tenants")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public class TenantsController : ControllerBase
{
    private readonly ITenantProvisionService _tenantProvisionService;

    public TenantsController(ITenantProvisionService tenantProvisionService)
    {
        _tenantProvisionService = tenantProvisionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTenants()
    {
        var result = await _tenantProvisionService.GetTenantsAsync();
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(GetMessage(result.Data, "Unable to load tenants.")));

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenants loaded successfully."));
    }

    [HttpGet("{tenantId:int}")]
    public async Task<IActionResult> GetTenantById(int tenantId)
    {
        var result = await _tenantProvisionService.GetTenantByIdAsync(tenantId);
        if (!result.Success)
            return NotFound(ApiResponse<object>.Fail(GetMessage(result.Data, "Tenant not found.")));

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenant details loaded successfully."));
    }

    [HttpPost("paged")]
    public async Task<IActionResult> GetPagedTenants([FromBody] TenantListRequest req)
    {
        var result = await _tenantProvisionService.GetTenantsPagedAsync(req);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(GetMessage(result.Data, "Unable to load tenant list.")));

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenant list loaded successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest req)
    {
        var result = await _tenantProvisionService.CreateTenantAsync(req);
        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = GetMessage(result.Data, "Tenant creation failed."),
                data = result.Data
            });
        }

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenant created successfully."));
    }

    [HttpPut("{tenantId}/admin/reset-password")]
    public async Task<IActionResult> ResetTenantAdminPassword(int tenantId, [FromBody] ResetTenantAdminPasswordRequest req)
    {
        var result = await _tenantProvisionService.ResetTenantAdminPasswordAsync(tenantId, req);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(GetMessage(result.Data, "Password reset failed.")));

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenant admin password reset successfully."));
    }

    [HttpPut("{tenantId}/admin/update-email")]
    public async Task<IActionResult> UpdateTenantAdminEmail(int tenantId, [FromBody] UpdateTenantAdminEmailRequest req)
    {
        var result = await _tenantProvisionService.UpdateTenantAdminEmailAsync(tenantId, req);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(GetMessage(result.Data, "Email update failed.")));

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenant admin email updated successfully."));
    }

    [HttpPut("{tenantId:int}/basic-info")]
    public async Task<IActionResult> UpdateTenantBasicInfo(int tenantId, [FromBody] UpdateTenantBasicInfoRequest req)
    {
        var result = await _tenantProvisionService.UpdateTenantBasicInfoAsync(tenantId, req);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(GetMessage(result.Data, "Tenant basic info update failed.")));

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenant basic info updated successfully."));
    }

    [HttpPut("{tenantId}/status")]
    public async Task<IActionResult> UpdateTenantStatus(int tenantId, [FromBody] UpdateTenantStatusRequest req)
    {
        var result = await _tenantProvisionService.UpdateTenantStatusAsync(tenantId, req);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(GetMessage(result.Data, "Tenant status update failed.")));

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenant status updated successfully."));
    }
    [HttpGet("{tenantId:int}/users")]
    public async Task<IActionResult> GetTenantUsers(int tenantId)
    {
        var result = await _tenantProvisionService.GetTenantUsersAsync(tenantId);

        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(GetMessage(result.Data, "Unable to load tenant users.")));

        return Ok(ApiResponse<object>.Ok(result.Data, "Tenant users loaded successfully."));
    }
    private static string GetMessage(object data, string fallback)
    {
        if (data is null)
            return fallback;

        var messageProperty = data.GetType().GetProperty("message") ?? data.GetType().GetProperty("Message");
        if (messageProperty?.GetValue(data)?.ToString() is string message && !string.IsNullOrWhiteSpace(message))
            return message;

        return fallback;
    }
}