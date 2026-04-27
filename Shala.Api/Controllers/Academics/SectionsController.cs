using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Academics;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;

namespace Shala.Api.Controllers.Academics;

[ApiController]
[Route("api/students/sections")]
public class SectionsController : TenantApiControllerBase
{
    private readonly ISectionService _service;

    public SectionsController(
        ISectionService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        var result = await _service.GetAllAsync(TenantId, branchId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        var result = await _service.GetByIdAsync(TenantId, branchId, id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSectionRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        var result = await _service.CreateAsync(TenantId, branchId, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateSectionRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        request.Id = id;

        var result = await _service.UpdateAsync(TenantId, branchId, Actor, request, cancellationToken);

        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);

        var result = await _service.DeleteAsync(TenantId, branchId, id, cancellationToken);

        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }
}