using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;
using Shala.Web.Repositories.Base;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.StudentDocuments;

public sealed class StudentDocumentChecklistWebRepository
    : RepositoryBase, IStudentDocumentChecklistWebRepository
{
    public StudentDocumentChecklistWebRepository(HttpClient httpClient, ApiSession session)
        : base(httpClient, session)
    {
    }

    public async Task<ApiResponse<StudentDocumentChecklistResponse>?> GetByAdmissionAsync(int studentAdmissionId)
    {
        await EnsureAuthAsync();

        var response = await HttpClient.GetAsync(
            $"api/student-document-checklist/admission/{studentAdmissionId}");

        return await ReadApiResponse<ApiResponse<StudentDocumentChecklistResponse>>(
            response,
            "Failed to load admission document checklist.");
    }

    public async Task<ApiResponse<StudentDocumentChecklistResponse>?> SaveAsync(
        SaveStudentDocumentChecklistRequest request)
    {
        await EnsureAuthAsync();

        var response = await HttpClient.PostAsJsonAsync(
            "api/student-document-checklist/save",
            request);

        return await ReadApiResponse<ApiResponse<StudentDocumentChecklistResponse>>(
            response,
            "Failed to save admission document checklist.");
    }

    public async Task<ApiResponse<StudentDocumentChecklistResponse>?> ValidateAsync(int studentAdmissionId)
    {
        await EnsureAuthAsync();

        var response = await HttpClient.GetAsync(
            $"api/student-document-checklist/admission/{studentAdmissionId}/validate");

        return await ReadApiResponse<ApiResponse<StudentDocumentChecklistResponse>>(
            response,
            "Failed to validate admission document checklist.");
    }
}