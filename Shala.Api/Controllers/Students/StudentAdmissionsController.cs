using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Students;
using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Api.Controllers.Students;

[Route("api/student-admissions")]
public class StudentAdmissionsController : TenantApiControllerBase
{
    private readonly IStudentAdmissionService _service;

    public StudentAdmissionsController(
        IStudentAdmissionService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudentAdmissionResponse>>> Create(
        [FromBody] CreateStudentAdmissionRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.CreateAsync(
            TenantId,
            branchId,
            Actor,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentAdmissionResponse>>> Update(
        int id,
        [FromBody] UpdateStudentAdmissionRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.UpdateAsync(
            TenantId,
            branchId,
            Actor,
            id,
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

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.DeleteAsync(
            TenantId,
            branchId,
            id,
            cancellationToken);

        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id:int}/assignment-detail")]
    public async Task<ActionResult<ApiResponse<SectionRollAssignmentDetailResponse>>> GetAssignmentDetail(
        int id,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.GetAssignmentDetailAsync(
            TenantId,
            branchId,
            id,
            cancellationToken);

        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id:int}/assignment-preview")]
    public async Task<ActionResult<ApiResponse<SectionRollAssignmentPreviewResponse>>> GetAssignmentPreview(
        int id,
        [FromQuery] int? sectionId,
        [FromQuery] bool autoGenerateRollNo,
        [FromQuery] string? rollNo,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.GetAssignmentPreviewAsync(
            TenantId,
            branchId,
            new SectionRollAssignmentPreviewRequest
            {
                StudentAdmissionId = id,
                SectionId = sectionId,
                AutoGenerateRollNo = autoGenerateRollNo,
                RollNo = rollNo
            },
            cancellationToken);

        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("assign-section-roll")]
    public async Task<ActionResult<ApiResponse<StudentAdmissionResponse>>> AssignSectionRoll(
        [FromBody] AssignSectionRollRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.AssignSectionAndRollNoAsync(
            TenantId,
            branchId,
            Actor,
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

    [HttpPost("bulk-assignment-preview")]
    public async Task<ActionResult<ApiResponse<BulkSectionRollAssignmentPreviewResponse>>> GetBulkAssignmentPreview(
        [FromBody] BulkSectionRollAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.GetBulkAssignmentPreviewAsync(
            TenantId,
            branchId,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("bulk-assign-section-roll")]
    public async Task<ActionResult<ApiResponse<List<StudentAdmissionResponse>>>> ApplyBulkAssignment(
        [FromBody] BulkSectionRollAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.ApplyBulkAssignmentAsync(
            TenantId,
            branchId,
            Actor,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("by-academic-year-class")]
    public async Task<ActionResult<ApiResponse<List<StudentAdmissionListItemResponse>>>> GetByAcademicYearAndClass(
        [FromQuery] int academicYearId,
        [FromQuery] int classId,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.GetAdmissionsByAcademicYearAndClassAsync(
            TenantId,
            branchId,
            academicYearId,
            classId,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}