using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.StudentDocument;
using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Api.Controllers.StudentDocument;

[ApiController]
[Route("api/student-document-models")]
public class StudentDocumentModelsController : TenantApiControllerBase
{
    private readonly IDocumentModelService _service;

    public StudentDocumentModelsController(
        IDocumentModelService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ApiResponse<List<DocumentModelResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.GetAllAsync(
            TenantId,
            branchId,
            cancellationToken);

        return ApiResponse<List<DocumentModelResponse>>.Ok(result);
    }

    [HttpGet("active")]
    public async Task<ApiResponse<List<DocumentModelResponse>>> GetActive(CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.GetActiveAsync(
            TenantId,
            branchId,
            cancellationToken);

        return ApiResponse<List<DocumentModelResponse>>.Ok(result);
    }

    [HttpPost]
    public async Task<ApiResponse<DocumentModelResponse>> Create(
        [FromBody] CreateDocumentModelRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.CreateAsync(
            TenantId,
            branchId,
            Actor,
            request,
            cancellationToken);

        return ApiResponse<DocumentModelResponse>.Ok(
            result,
            "Checklist document created successfully.");
    }

    [HttpPut("{id:int}")]
    public async Task<ApiResponse<DocumentModelResponse>> Update(
        int id,
        [FromBody] UpdateDocumentModelRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        request.Id = id;

        var result = await _service.UpdateAsync(
            TenantId,
            branchId,
            Actor,
            request,
            cancellationToken);

        return ApiResponse<DocumentModelResponse>.Ok(
            result,
            "Checklist document updated successfully.");
    }
}