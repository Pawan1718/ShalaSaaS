using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.StudentDocument;
using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Api.Controllers.StudentDocument;

[ApiController]
[Route("api/student-document-checklist")]
public class StudentDocumentChecklistController : TenantApiControllerBase
{
    private readonly IStudentDocumentChecklistService _service;

    public StudentDocumentChecklistController(
        IStudentDocumentChecklistService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet("admission/{admissionId:int}")]
    public async Task<ApiResponse<StudentDocumentChecklistResponse>> GetByAdmission(
        int admissionId,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.GetByAdmissionAsync(
            TenantId,
            branchId,
            admissionId,
            cancellationToken);

        return ApiResponse<StudentDocumentChecklistResponse>.Ok(result);
    }

    [HttpPost("save")]
    public async Task<ApiResponse<StudentDocumentChecklistResponse>> Save(
        [FromBody] SaveStudentDocumentChecklistRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.SaveAsync(
            TenantId,
            branchId,
            Actor,
            request,
            cancellationToken);

        return ApiResponse<StudentDocumentChecklistResponse>.Ok(
            result,
            "Student document checklist saved successfully.");
    }

    [HttpGet("admission/{admissionId:int}/validate")]
    public async Task<ApiResponse<StudentDocumentChecklistResponse>> Validate(
        int admissionId,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.ValidateAsync(
            TenantId,
            branchId,
            admissionId,
            cancellationToken);

        var message = result.IsValid
            ? "Document checklist is valid."
            : "Required documents are pending.";

        return ApiResponse<StudentDocumentChecklistResponse>.Ok(result, message);
    }
}