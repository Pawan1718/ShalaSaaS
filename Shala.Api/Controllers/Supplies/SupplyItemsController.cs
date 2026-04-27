using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Supplies;
using Shala.Shared.Requests.Supplies;

namespace Shala.Api.Controllers.Supplies;

[ApiController]
[Route("api/tenant/supplies/items")]
public class SupplyItemsController : TenantApiControllerBase
{
    private readonly IStudentSupplyService _service;

    public SupplyItemsController(
        IStudentSupplyService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        var safeBranchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        var data = await _service.GetItemsAsync(
            TenantId,
            safeBranchId,
            activeOnly,
            cancellationToken);

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSupplyItemRequest request,
        CancellationToken cancellationToken)
    {
        var safeBranchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        var result = await _service.CreateItemAsync(
            TenantId,
            safeBranchId,
            Actor,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateSupplyItemRequest request,
        CancellationToken cancellationToken)
    {
        var safeBranchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        var result = await _service.UpdateItemAsync(
            TenantId,
            safeBranchId,
            Actor,
            id,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var safeBranchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        var result = await _service.DeleteItemAsync(
            TenantId,
            safeBranchId,
            id,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}