using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.Academics;
using Shala.Shared.Requests.Academics;

namespace Shala.Api.Controllers.Academics;

[Route("api/students/classes")]
public class AcademicClassesController : TenantApiControllerBase
{
    private readonly IAcademicClassService _service;

    public AcademicClassesController(
        IAcademicClassService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(TenantId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(TenantId, id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAcademicClassRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(TenantId, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateAcademicClassRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;

        var result = await _service.UpdateAsync(TenantId, Actor, request, cancellationToken);

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
        var result = await _service.DeleteAsync(TenantId, id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}