using Shala.Domain.Entities.StudentDocuments;

namespace Shala.Application.Features.StudentDocument
{
    internal static class StudentDocumentValidation
    {
        public static void ValidateUploadRequest(Shared.Requests.StudentDocument.UploadStudentDocumentRequest request)
        {
            if (request.StudentId <= 0)
                throw new InvalidOperationException("Student is required.");

            if (request.File is null || request.File.Length == 0)
                throw new InvalidOperationException("Document file is required.");
        }

        public static void ValidateAgainstModel(DocumentModel model, Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (!model.IsActive)
                throw new InvalidOperationException("Selected document model is inactive.");

            if (!string.IsNullOrWhiteSpace(model.AllowedFileTypes))
            {
                var allowed = model.AllowedFileTypes
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(x => x.Trim().TrimStart('.').ToLowerInvariant())
                    .ToHashSet();

                var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();

                if (!allowed.Contains(extension))
                    throw new InvalidOperationException($"Invalid file type. Allowed: {model.AllowedFileTypes}");
            }

            if (model.MaxFileSizeInKb.HasValue)
            {
                var maxBytes = model.MaxFileSizeInKb.Value * 1024L;
                if (file.Length > maxBytes)
                    throw new InvalidOperationException(
                        $"File size exceeds allowed limit of {model.MaxFileSizeInKb.Value} KB.");
            }
        }

        public static string NormalizeCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Document model code is required.");

            return code.Trim().ToUpperInvariant();
        }

        public static string NormalizeRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{fieldName} is required.");

            return value.Trim();
        }
    }
}