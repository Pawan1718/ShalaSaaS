using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;
using Shala.Web.Repositories.Base;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.StudentDocuments;

public sealed class DocumentModelWebRepository : RepositoryBase, IDocumentModelWebRepository
{
    public DocumentModelWebRepository(HttpClient httpClient, ApiSession session)
        : base(httpClient, session)
    {
    }

    public async Task<ApiResponse<List<DocumentModelResponse>>?> GetAllAsync()
    {
        await EnsureAuthAsync();

        var response = await HttpClient.GetAsync("api/student-document-models");

        return await ReadApiResponse<ApiResponse<List<DocumentModelResponse>>>(
            response,
            "Failed to load document checklist config.");
    }

    public async Task<ApiResponse<List<DocumentModelResponse>>?> GetActiveAsync()
    {
        await EnsureAuthAsync();

        var response = await HttpClient.GetAsync("api/student-document-models/active");

        return await ReadApiResponse<ApiResponse<List<DocumentModelResponse>>>(
            response,
            "Failed to load active checklist documents.");
    }

    public async Task<ApiResponse<DocumentModelResponse>?> CreateAsync(CreateDocumentModelRequest request)
    {
        await EnsureAuthAsync();

        var response = await HttpClient.PostAsJsonAsync("api/student-document-models", request);

        return await ReadApiResponse<ApiResponse<DocumentModelResponse>>(
            response,
            "Failed to create checklist document.");
    }

    public async Task<ApiResponse<DocumentModelResponse>?> UpdateAsync(UpdateDocumentModelRequest request)
    {
        await EnsureAuthAsync();

        var response = await HttpClient.PutAsJsonAsync(
            $"api/student-document-models/{request.Id}",
            request);

        return await ReadApiResponse<ApiResponse<DocumentModelResponse>>(
            response,
            "Failed to update checklist document.");
    }
}