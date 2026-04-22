using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.Academics;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Requests.TenantConfigSetting;

namespace Shala.Api.Controllers.Academics;

[Route("api/students/academic-years")]
public class AcademicYearsController : TenantApiControllerBase
{
    private readonly IAcademicYearService _service;

    public AcademicYearsController(
        IAcademicYearService service,
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
        [FromBody] CreateAcademicYearRequest request,
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
        [FromBody] UpdateAcademicYearRequest request,
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
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id:int}/set-current")]
    public async Task<IActionResult> SetCurrent(int id, CancellationToken cancellationToken)
    {
        var result = await _service.SetCurrentAsync(TenantId, id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("ensure-next")]
    public async Task<IActionResult> EnsureNext(CancellationToken cancellationToken)
    {
        var result = await _service.EnsureNextAcademicYearAsync(TenantId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
    {
        var result = await _service.GetSettingsAsync(TenantId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("settings")]
    public async Task<IActionResult> SaveSettings(
        [FromBody] SaveAcademicYearSettingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SaveSettingsAsync(TenantId, Actor, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}