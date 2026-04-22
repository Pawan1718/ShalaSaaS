using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument
{
    public interface IStudentDocumentService
    {
        Task<StudentDocumentResponse> UploadAsync(int tenantId, int branchId, string actor, UploadStudentDocumentRequest request, CancellationToken cancellationToken = default);
        Task<List<StudentDocumentResponse>> GetByStudentIdAsync(int tenantId, int branchId, int studentId, CancellationToken cancellationToken = default);
        Task<StudentDocumentResponse?> GetByIdAsync(int tenantId, int branchId, int id, CancellationToken cancellationToken = default);
        Task<StudentDocumentResponse> UpdateAsync(int tenantId, int branchId, string actor, UpdateStudentDocumentRequest request, CancellationToken cancellationToken = default);
        Task<StudentDocumentAnalysisResponse> AnalyzeAsync(int tenantId, int branchId, string actor, AnalyzeStudentDocumentRequest request, CancellationToken cancellationToken = default);
        Task<StudentDocumentResponse> VerifyAsync(int tenantId, int branchId, string actor, VerifyStudentDocumentRequest request, CancellationToken cancellationToken = default);
        Task<StudentDocumentResponse> ToggleStatusAsync(int tenantId, int branchId, string actor, ToggleStudentDocumentStatusRequest request, CancellationToken cancellationToken = default);
    }
}