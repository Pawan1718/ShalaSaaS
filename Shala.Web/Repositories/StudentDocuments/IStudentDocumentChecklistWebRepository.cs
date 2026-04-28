using Shala.Shared.Common;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Web.Repositories.StudentDocuments;

public interface IStudentDocumentChecklistWebRepository
{
    Task<ApiResponse<StudentDocumentChecklistResponse>?> GetByAdmissionAsync(int studentAdmissionId);
    Task<ApiResponse<StudentDocumentChecklistResponse>?> SaveAsync(SaveStudentDocumentChecklistRequest request);
    Task<ApiResponse<StudentDocumentChecklistResponse>?> ValidateAsync(int studentAdmissionId);
}