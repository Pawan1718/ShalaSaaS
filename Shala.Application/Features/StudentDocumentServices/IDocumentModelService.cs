using Shala.Shared.Responses.StudentDocument;
using Shala.Shared.Requests.StudentDocument;

namespace Shala.Application.Features.StudentDocument
{
    public interface IDocumentModelService
    {
        Task<List<DocumentModelResponse>> GetAllAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);
        Task<List<DocumentModelResponse>> GetActiveAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);
        Task<DocumentModelResponse?> GetByIdAsync(int tenantId, int branchId, int id, CancellationToken cancellationToken = default);
        Task<DocumentModelResponse> CreateAsync(int tenantId, int branchId, string actor, CreateDocumentModelRequest request, CancellationToken cancellationToken = default);
        Task<DocumentModelResponse> UpdateAsync(int tenantId, int branchId, string actor, UpdateDocumentModelRequest request, CancellationToken cancellationToken = default);
        Task<DocumentModelResponse> ToggleStatusAsync(int tenantId, int branchId, string actor, ToggleDocumentModelStatusRequest request, CancellationToken cancellationToken = default);
    }
}