using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Students;
using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;

namespace Shala.Api.Controllers.Students;

[Route("api/students")]
public class StudentsController : TenantApiControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(
        IStudentService studentService,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _studentService = studentService;
    }

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<StudentListItemResponse>>>> Search(
        [FromBody] StudentListRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _studentService.GetPagedAsync(
            TenantId,
            branchId,
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentDetailsResponse>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _studentService.GetByIdAsync(
            TenantId,
            branchId,
            id,
            cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudentDetailsResponse>>> Create(
        [FromBody] CreateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _studentService.CreateAsync(
            TenantId,
            branchId,
            Actor,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentDetailsResponse>>> Update(
        int id,
        [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _studentService.UpdateAsync(
            TenantId,
            branchId,
            Actor,
            id,
            request,
            cancellationToken);

        if (!result.Success)
        {
            if (result.Message == "Student not found")
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }
}