using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.Students;
using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;

namespace Shala.Api.Controllers.Students;

[ApiController]
[Route("api/students/{studentId:int}/guardians")]
public class StudentGuardiansController : TenantApiControllerBase
{
    private readonly IStudentGuardianService _studentGuardianService;

    public StudentGuardiansController(
        IStudentGuardianService studentGuardianService,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _studentGuardianService = studentGuardianService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GuardianResponse>>> Add(
        int studentId,
        [FromBody] CreateGuardianRequest request,
        CancellationToken cancellationToken)
    {
        var safeBranchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);
        var result = await _studentGuardianService.AddAsync(
            TenantId,
            safeBranchId,
            Actor,
            studentId,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{guardianId:int}")]
    public async Task<ActionResult<ApiResponse<GuardianResponse>>> Update(
        int studentId,
        int guardianId,
        [FromBody] CreateGuardianRequest request,
        CancellationToken cancellationToken)
    {
        var safeBranchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);
        var result = await _studentGuardianService.UpdateAsync(
            TenantId,
            safeBranchId,
            Actor,
            studentId,
            guardianId,
            request,
            cancellationToken);

        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{guardianId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Remove(
        int studentId,
        int guardianId,
        CancellationToken cancellationToken)
    {
        var safeBranchId = await GetSafeBranchIdAsync(BranchId, cancellationToken);
        var result = await _studentGuardianService.RemoveAsync(
            TenantId,
            safeBranchId,
            studentId,
            guardianId,
            cancellationToken);

        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }
}