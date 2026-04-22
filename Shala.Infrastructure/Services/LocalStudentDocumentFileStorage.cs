using Microsoft.AspNetCore.Http;
using Shala.Application.Features.StudentDocument;

namespace Shala.Infrastructure.Storage
{
    public sealed class LocalStudentDocumentFileStorage : IStudentDocumentFileStorage
    {
        public async Task<StudentDocumentStoredFileResult> SaveAsync(
            int tenantId,
            int branchId,
            int studentId,
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            var uploadsRoot = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "student-documents",
                tenantId.ToString(),
                branchId.ToString(),
                studentId.ToString());

            Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(file.FileName);
            var storedFileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(uploadsRoot, storedFileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return new StudentDocumentStoredFileResult
            {
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                FilePath = fullPath.Replace("\\", "/"),
                MimeType = file.ContentType,
                FileSize = file.Length
            };
        }
    }
}