using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Web.Repositories.StudentDocuments;

public interface IDocumentModelWebRepository
{
    Task<ApiResponse<List<DocumentModelResponse>>?> GetAllAsync();
    Task<ApiResponse<List<DocumentModelResponse>>?> GetActiveAsync();
    Task<ApiResponse<DocumentModelResponse>?> CreateAsync(CreateDocumentModelRequest request);
    Task<ApiResponse<DocumentModelResponse>?> UpdateAsync(UpdateDocumentModelRequest request);
}