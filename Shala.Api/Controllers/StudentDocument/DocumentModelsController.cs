using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.StudentDocument;
using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Api.Controllers.StudentDocument
{
    [ApiController]
    [Route("api/student-document-models")]
    public class DocumentModelsController : TenantApiControllerBase
    {
        private readonly IDocumentModelService _service;

        public DocumentModelsController(
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
            var result = await _service.GetAllAsync(TenantId, BranchId, cancellationToken);
            return ApiResponse<List<DocumentModelResponse>>.Ok(result);
        }

        [HttpGet("active")]
        public async Task<ApiResponse<List<DocumentModelResponse>>> GetActive(CancellationToken cancellationToken)
        {
            var result = await _service.GetActiveAsync(TenantId, BranchId, cancellationToken);
            return ApiResponse<List<DocumentModelResponse>>.Ok(result);
        }

        [HttpPost]
        public async Task<ApiResponse<DocumentModelResponse>> Create([FromBody] CreateDocumentModelRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(TenantId, BranchId, Actor, request, cancellationToken);
            return ApiResponse<DocumentModelResponse>.Ok(result, "Document model created successfully.");
        }

        [HttpPut]
        public async Task<ApiResponse<DocumentModelResponse>> Update([FromBody] UpdateDocumentModelRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateAsync(TenantId, BranchId, Actor, request, cancellationToken);
            return ApiResponse<DocumentModelResponse>.Ok(result, "Document model updated successfully.");
        }

        [HttpPost("toggle-status")]
        public async Task<ApiResponse<DocumentModelResponse>> ToggleStatus([FromBody] ToggleDocumentModelStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.ToggleStatusAsync(TenantId, BranchId, Actor, request, cancellationToken);
            return ApiResponse<DocumentModelResponse>.Ok(result, "Document model status updated successfully.");
        }
    }
}