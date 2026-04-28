using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument;

public interface IDocumentModelService
{
    Task<List<DocumentModelResponse>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<List<DocumentModelResponse>> GetActiveAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<DocumentModelResponse> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        CreateDocumentModelRequest request,
        CancellationToken cancellationToken = default);

    Task<DocumentModelResponse> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        UpdateDocumentModelRequest request,
        CancellationToken cancellationToken = default);
}