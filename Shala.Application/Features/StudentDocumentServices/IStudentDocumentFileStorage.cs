using Microsoft.AspNetCore.Http;

namespace Shala.Application.Features.StudentDocument
{
    public interface IStudentDocumentFileStorage
    {
        Task<StudentDocumentStoredFileResult> SaveAsync(
            int tenantId,
            int branchId,
            int studentId,
            IFormFile file,
            CancellationToken cancellationToken = default);
    }

    public sealed class StudentDocumentStoredFileResult
    {
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}