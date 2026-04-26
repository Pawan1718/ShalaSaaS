using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shala.Application.Features.Platform;
using Shala.Shared.Common;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;

namespace Shala.Api.Controllers.Platform;

[ApiController]
[Authorize(Roles = "SuperAdmin")]
[Route("api/platform/tenants/{tenantId:int}/branches")]
public class BranchesController : ControllerBase
{
    private readonly IBranchService _branchService;

    public BranchesController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<BranchResponse>>>> GetAll(int tenantId, CancellationToken cancellationToken)
    {
        var result = await _branchService.GetAllAsync(tenantId, cancellationToken);

        if (!result.Success)
            return BadRequest(ApiResponse<List<BranchResponse>>.Fail(result.Message ?? "Unable to load branches."));

        return Ok(ApiResponse<List<BranchResponse>>.Ok(result.Data, result.Message ?? "Branches loaded successfully."));
    }

    [HttpGet("{branchId:int}")]
    public async Task<ActionResult<ApiResponse<BranchResponse>>> GetById(int tenantId, int branchId, CancellationToken cancellationToken)
    {
        var result = await _branchService.GetByIdAsync(tenantId, branchId, cancellationToken);

        if (!result.Success || result.Data is null)
            return NotFound(ApiResponse<BranchResponse>.Fail(result.Message ?? "Branch not found."));

        return Ok(ApiResponse<BranchResponse>.Ok(result.Data, result.Message ?? "Branch details loaded successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BranchResponse>>> Create(int tenantId, [FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
    {
        request.TenantId = tenantId;
        var result = await _branchService.CreateAsync(request, cancellationToken);

        if (!result.Success || result.Data is null)
            return BadRequest(ApiResponse<BranchResponse>.Fail(result.Message ?? "Branch creation failed."));

        return CreatedAtAction(nameof(GetById), new { tenantId, branchId = result.Data.Id }, ApiResponse<BranchResponse>.Ok(result.Data, result.Message ?? "Branch created successfully."));
    }

    [HttpPut("{branchId:int}")]
    public async Task<ActionResult<ApiResponse<BranchResponse>>> Update(int tenantId, int branchId, [FromBody] UpdateBranchRequest request, CancellationToken cancellationToken)
    {
        var result = await _branchService.UpdateAsync(tenantId, branchId, request, cancellationToken);

        if (!result.Success)
        {
            if (result.Message == "Branch not found.")
                return NotFound(ApiResponse<BranchResponse>.Fail(result.Message));

            return BadRequest(ApiResponse<BranchResponse>.Fail(result.Message ?? "Branch update failed."));
        }

        return Ok(ApiResponse<BranchResponse>.Ok(result.Data, result.Message ?? "Branch updated successfully."));
    }

    [HttpDelete("{branchId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int tenantId, int branchId, CancellationToken cancellationToken)
    {
        var result = await _branchService.DeleteAsync(tenantId, branchId, cancellationToken);

        if (!result.Success)
        {
            if (result.Message == "Branch not found.")
                return NotFound(ApiResponse<object>.Fail(result.Message));

            return BadRequest(ApiResponse<object>.Fail(result.Message ?? "Branch deletion failed."));
        }

        return Ok(ApiResponse<object>.Ok(result.Data, result.Message ?? "Branch deleted successfully."));
    }
}