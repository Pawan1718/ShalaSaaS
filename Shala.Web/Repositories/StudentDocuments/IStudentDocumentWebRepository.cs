using Microsoft.AspNetCore.Components.Forms;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Web.Repositories.StudentDocuments
{
    public interface IStudentDocumentWebRepository
    {
        Task<List<StudentDocumentResponse>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
        Task<StudentDocumentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<DocumentModelResponse>> GetActiveModelsAsync(CancellationToken cancellationToken = default);

        Task<StudentDocumentResponse> UploadAsync(UploadStudentDocumentWebRequest request, CancellationToken cancellationToken = default);
        Task<StudentDocumentResponse> UpdateAsync(UpdateStudentDocumentRequest request, CancellationToken cancellationToken = default);
        Task<StudentDocumentAnalysisResponse> AnalyzeAsync(AnalyzeStudentDocumentRequest request, CancellationToken cancellationToken = default);
        Task<StudentDocumentResponse> VerifyAsync(VerifyStudentDocumentRequest request, CancellationToken cancellationToken = default);
        Task<StudentDocumentResponse> ToggleStatusAsync(ToggleStudentDocumentStatusRequest request, CancellationToken cancellationToken = default);
    }

    public class UploadStudentDocumentWebRequest
    {
        public int StudentId { get; set; }
        public int? StudentRegistrationId { get; set; }
        public int? StudentAdmissionId { get; set; }
        public int? DocumentModelId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public IBrowserFile File { get; set; } = null!;
    }
}