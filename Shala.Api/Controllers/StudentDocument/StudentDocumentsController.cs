using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.StudentDocument;
using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Api.Controllers.StudentDocument
{
    [ApiController]
    [Route("api/student-documents")]
    public class StudentDocumentsController : TenantApiControllerBase
    {
        private readonly IStudentDocumentService _service;
        private readonly IDocumentModelService _documentModelService;

        public StudentDocumentsController(
            IStudentDocumentService service,
            IDocumentModelService documentModelService,
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
            : base(currentUser, accessScopeValidator)
        {
            _service = service;
            _documentModelService = documentModelService;
        }

        [HttpGet("student/{studentId:int}")]
        public async Task<ApiResponse<List<StudentDocumentResponse>>> GetByStudentId(int studentId, CancellationToken cancellationToken)
        {
            var result = await _service.GetByStudentIdAsync(TenantId, BranchId, studentId, cancellationToken);
            return ApiResponse<List<StudentDocumentResponse>>.Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ApiResponse<StudentDocumentResponse?>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(TenantId, BranchId, id, cancellationToken);
            return ApiResponse<StudentDocumentResponse?>.Ok(result);
        }

        [HttpGet("active-models")]
        public async Task<ApiResponse<List<DocumentModelResponse>>> GetActiveModels(CancellationToken cancellationToken)
        {
            var result = await _documentModelService.GetActiveAsync(TenantId, BranchId, cancellationToken);
            return ApiResponse<List<DocumentModelResponse>>.Ok(result);
        }

        [HttpPost("upload")]
        public async Task<ApiResponse<StudentDocumentResponse>> Upload([FromForm] UploadStudentDocumentRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.UploadAsync(TenantId, BranchId, Actor, request, cancellationToken);
            return ApiResponse<StudentDocumentResponse>.Ok(result, "Student document uploaded successfully.");
        }

        [HttpPut]
        public async Task<ApiResponse<StudentDocumentResponse>> Update([FromBody] UpdateStudentDocumentRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateAsync(TenantId, BranchId, Actor, request, cancellationToken);
            return ApiResponse<StudentDocumentResponse>.Ok(result, "Student document updated successfully.");
        }

        [HttpPost("analyze")]
        public async Task<ApiResponse<StudentDocumentAnalysisResponse>> Analyze([FromBody] AnalyzeStudentDocumentRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.AnalyzeAsync(TenantId, BranchId, Actor, request, cancellationToken);
            return ApiResponse<StudentDocumentAnalysisResponse>.Ok(result, "Student document analyzed successfully.");
        }

        [HttpPost("verify")]
        public async Task<ApiResponse<StudentDocumentResponse>> Verify([FromBody] VerifyStudentDocumentRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.VerifyAsync(TenantId, BranchId, Actor, request, cancellationToken);
            return ApiResponse<StudentDocumentResponse>.Ok(result, "Student document verification updated successfully.");
        }

        [HttpPost("toggle-status")]
        public async Task<ApiResponse<StudentDocumentResponse>> ToggleStatus([FromBody] ToggleStudentDocumentStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.ToggleStatusAsync(TenantId, BranchId, Actor, request, cancellationToken);
            return ApiResponse<StudentDocumentResponse>.Ok(result, "Student document status updated successfully.");
        }
    }
}