using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.TenantConfig;
using Shala.Shared.Requests.Students;

namespace Shala.Api.Controllers.TenantConfig;

[Route("api/students/roll-number-settings")]
public class RollNumberSettingsController : TenantApiControllerBase
{
    private readonly IRollNumberSettingService _service;

    public RollNumberSettingsController(
        IRollNumberSettingService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(TenantId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save(
        [FromBody] SaveRollNumberSettingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SaveAsync(TenantId, Actor, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}