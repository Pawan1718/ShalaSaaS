using Shala.Domain.Entities.StudentDocuments;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument
{
    public static class StudentDocumentMappings
    {
        public static DocumentModelResponse ToResponse(this DocumentModel entity)
        {
            return new DocumentModelResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                Description = entity.Description,
                IsRequired = entity.IsRequired,
                IsAiValidationEnabled = entity.IsAiValidationEnabled,
                BlockAdmissionOnMismatch = entity.BlockAdmissionOnMismatch,
                IsActive = entity.IsActive,
                AllowedFileTypes = entity.AllowedFileTypes,
                MaxFileSizeInKb = entity.MaxFileSizeInKb,
                RequiredFieldsJson = entity.RequiredFieldsJson,
                DisplayOrder = entity.DisplayOrder,
                CreatedAt = entity.CreatedAt
            };
        }

        public static StudentDocumentResponse ToResponse(this Domain.Entities.StudentDocuments.StudentDocument entity)
        {
            return new StudentDocumentResponse
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                StudentRegistrationId = entity.StudentRegistrationId,
                StudentAdmissionId = entity.StudentAdmissionId,
                DocumentModelId = entity.DocumentModelId,
                DocumentType = entity.DocumentType,
                Title = entity.Title,
                FileName = entity.FileName,
                FilePath = entity.FilePath,
                MimeType = entity.MimeType,
                FileSize = entity.FileSize,
                IsRequired = entity.IsRequired,
                IsVerified = entity.IsVerified,
                IsActive = entity.IsActive,
                Status = entity.Status,
                Remarks = entity.Remarks,
                CreatedAt = entity.CreatedAt
            };
        }

        public static StudentDocumentAnalysisResponse ToResponse(this StudentDocumentAnalysis entity)
        {
            return new StudentDocumentAnalysisResponse
            {
                StudentDocumentId = entity.StudentDocumentId,
                ExtractedText = entity.ExtractedText,
                ExtractedJson = entity.ExtractedJson,
                DetectedDocumentType = entity.DetectedDocumentType,
                OcrConfidence = entity.OcrConfidence,
                AiConfidence = entity.AiConfidence,
                AnalysisStatus = entity.AnalysisStatus,
                FieldMatches = entity.FieldMatches.Select(x => new StudentDocumentFieldMatchResponse
                {
                    FieldName = x.FieldName,
                    DocumentValue = x.DocumentValue,
                    FormValue = x.FormValue,
                    MatchStatus = x.MatchStatus,
                    ConfidenceScore = x.ConfidenceScore,
                    IsCritical = x.IsCritical,
                    Suggestion = x.Suggestion
                }).ToList()
            };
        }
    }
}