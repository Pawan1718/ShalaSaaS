using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Web.Repositories.StudentDocuments
{
    public interface IDocumentModelWebRepository
    {
        Task<List<DocumentModelResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<DocumentModelResponse>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<DocumentModelResponse> CreateAsync(CreateDocumentModelRequest request, CancellationToken cancellationToken = default);
        Task<DocumentModelResponse> UpdateAsync(UpdateDocumentModelRequest request, CancellationToken cancellationToken = default);
        Task<DocumentModelResponse> ToggleStatusAsync(ToggleDocumentModelStatusRequest request, CancellationToken cancellationToken = default);
    }
}