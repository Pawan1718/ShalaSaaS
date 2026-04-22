using Shala.Application.Common;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Domain.Constants;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument
{
    public sealed class StudentDocumentService : IStudentDocumentService
    {
        private readonly IStudentDocumentRepository _repo;
        private readonly IDocumentModelRepository _documentModelRepository;
        private readonly IStudentDocumentFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;

        public StudentDocumentService(
            IStudentDocumentRepository repo,
            IDocumentModelRepository documentModelRepository,
            IStudentDocumentFileStorage fileStorage,
            IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _documentModelRepository = documentModelRepository;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentDocumentResponse> UploadAsync(
            int tenantId,
            int branchId,
            string actor,
            UploadStudentDocumentRequest request,
            CancellationToken cancellationToken = default)
        {
            StudentDocumentValidation.ValidateUploadRequest(request);

            DocumentModel? model = null;

            if (request.DocumentModelId.HasValue)
            {
                model = await _documentModelRepository.FirstOrDefaultAsync(
                    x => x.Id == request.DocumentModelId.Value &&
                         x.TenantId == tenantId &&
                         x.BranchId == branchId,
                    cancellationToken);

                if (model is null)
                    throw new KeyNotFoundException("Document model not found.");

                StudentDocumentValidation.ValidateAgainstModel(model, request.File);
            }

            var stored = await _fileStorage.SaveAsync(
                tenantId,
                branchId,
                request.StudentId,
                request.File,
                cancellationToken);

            var entity = new Domain.Entities.StudentDocuments.StudentDocument
            {
                TenantId = tenantId,
                BranchId = branchId,
                StudentId = request.StudentId,
                StudentRegistrationId = request.StudentRegistrationId,
                StudentAdmissionId = request.StudentAdmissionId,
                DocumentModelId = request.DocumentModelId,
                DocumentType = !string.IsNullOrWhiteSpace(request.DocumentType)
                    ? request.DocumentType.Trim()
                    : model?.Code ?? string.Empty,
                Title = !string.IsNullOrWhiteSpace(request.Title)
                    ? request.Title.Trim()
                    : model?.Name ?? request.File.FileName,
                FileName = stored.OriginalFileName,
                FilePath = stored.FilePath,
                MimeType = stored.MimeType,
                FileSize = stored.FileSize,
                IsRequired = request.IsRequired || (model?.IsRequired ?? false),
                IsVerified = false,
                IsActive = true,
                Status = StudentDocumentStatuses.Uploaded,
                Remarks = null,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = actor
            };

            await _repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.ToResponse();
        }

        public async Task<List<StudentDocumentResponse>> GetByStudentIdAsync(
            int tenantId,
            int branchId,
            int studentId,
            CancellationToken cancellationToken = default)
        {
            var items = await _repo.GetByStudentIdAsync(studentId, tenantId, branchId, cancellationToken);
            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task<StudentDocumentResponse?> GetByIdAsync(
            int tenantId,
            int branchId,
            int id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetScopedByIdAsync(id, tenantId, branchId, cancellationToken);
            return entity?.ToResponse();
        }

        public async Task<StudentDocumentResponse> UpdateAsync(
            int tenantId,
            int branchId,
            string actor,
            UpdateStudentDocumentRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetScopedByIdAsync(request.Id, tenantId, branchId, cancellationToken)
                ?? throw new KeyNotFoundException("Student document not found.");

            entity.DocumentType = StudentDocumentValidation.NormalizeRequired(request.DocumentType, "Document type");
            entity.Title = StudentDocumentValidation.NormalizeRequired(request.Title, "Title");
            entity.IsRequired = request.IsRequired;
            entity.Remarks = string.IsNullOrWhiteSpace(request.Remarks) ? null : request.Remarks.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = actor;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.ToResponse();
        }

        public async Task<StudentDocumentAnalysisResponse> AnalyzeAsync(
            int tenantId,
            int branchId,
            string actor,
            AnalyzeStudentDocumentRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetScopedByIdAsync(request.StudentDocumentId, tenantId, branchId, cancellationToken)
                ?? throw new KeyNotFoundException("Student document not found.");

            if (entity.Analysis is null)
            {
                entity.Analysis = new StudentDocumentAnalysis
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    StudentDocumentId = entity.Id,
                    ExtractedText = null,
                    ExtractedJson = null,
                    DetectedDocumentType = entity.DocumentType,
                    OcrConfidence = null,
                    AiConfidence = null,
                    AnalysisStatus = StudentDocumentAnalysisStatuses.Pending,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = actor
                };
            }
            else
            {
                if (!request.ForceReAnalyze &&
                    entity.Analysis.AnalysisStatus == StudentDocumentAnalysisStatuses.Completed)
                {
                    return new StudentDocumentAnalysisResponse
                    {
                        StudentDocumentId = entity.Id,
                        ExtractedText = entity.Analysis.ExtractedText,
                        ExtractedJson = entity.Analysis.ExtractedJson,
                        DetectedDocumentType = entity.Analysis.DetectedDocumentType,
                        OcrConfidence = entity.Analysis.OcrConfidence,
                        AiConfidence = entity.Analysis.AiConfidence,
                        AnalysisStatus = entity.Analysis.AnalysisStatus,
                        FieldMatches = entity.Analysis.FieldMatches.Select(x => new StudentDocumentFieldMatchResponse
                        {
                            FieldName = x.FieldName,
                            DocumentValue = x.DocumentValue,
                            FormValue = x.FormValue,
                            MatchStatus = x.MatchStatus,
                            ConfidenceScore = x.ConfidenceScore,
                            IsCritical = x.IsCritical,
                            Suggestion = x.Suggestion
                        }).ToList(),
                        Suggestions = entity.Suggestions.Select(x => new StudentDocumentSuggestionResponse
                        {
                            Id = x.Id,
                            SuggestionType = x.SuggestionType,
                            Message = x.Message,
                            SuggestedValue = x.SuggestedValue,
                            ConfidenceScore = x.ConfidenceScore,
                            IsApplied = x.IsApplied,
                            IsDismissed = x.IsDismissed
                        }).ToList()
                    };
                }

                entity.Analysis.DetectedDocumentType = entity.DocumentType;
                entity.Analysis.AnalysisStatus = StudentDocumentAnalysisStatuses.Pending;
                entity.Analysis.UpdatedAt = DateTime.UtcNow;
                entity.Analysis.UpdatedBy = actor;
            }

            entity.Status = StudentDocumentStatuses.Analyzed;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = actor;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StudentDocumentAnalysisResponse
            {
                StudentDocumentId = entity.Id,
                ExtractedText = entity.Analysis.ExtractedText,
                ExtractedJson = entity.Analysis.ExtractedJson,
                DetectedDocumentType = entity.Analysis.DetectedDocumentType,
                OcrConfidence = entity.Analysis.OcrConfidence,
                AiConfidence = entity.Analysis.AiConfidence,
                AnalysisStatus = entity.Analysis.AnalysisStatus,
                FieldMatches = entity.Analysis.FieldMatches.Select(x => new StudentDocumentFieldMatchResponse
                {
                    FieldName = x.FieldName,
                    DocumentValue = x.DocumentValue,
                    FormValue = x.FormValue,
                    MatchStatus = x.MatchStatus,
                    ConfidenceScore = x.ConfidenceScore,
                    IsCritical = x.IsCritical,
                    Suggestion = x.Suggestion
                }).ToList(),
                Suggestions = entity.Suggestions.Select(x => new StudentDocumentSuggestionResponse
                {
                    Id = x.Id,
                    SuggestionType = x.SuggestionType,
                    Message = x.Message,
                    SuggestedValue = x.SuggestedValue,
                    ConfidenceScore = x.ConfidenceScore,
                    IsApplied = x.IsApplied,
                    IsDismissed = x.IsDismissed
                }).ToList()
            };
        }

        public async Task<StudentDocumentResponse> VerifyAsync(
            int tenantId,
            int branchId,
            string actor,
            VerifyStudentDocumentRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetScopedByIdAsync(request.StudentDocumentId, tenantId, branchId, cancellationToken)
                ?? throw new KeyNotFoundException("Student document not found.");

            entity.IsVerified = request.IsVerified;
            entity.Remarks = string.IsNullOrWhiteSpace(request.Remarks) ? null : request.Remarks.Trim();
            entity.Status = request.IsVerified
                ? StudentDocumentStatuses.Verified
                : StudentDocumentStatuses.NeedsReview;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = actor;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.ToResponse();
        }

        public async Task<StudentDocumentResponse> ToggleStatusAsync(
            int tenantId,
            int branchId,
            string actor,
            ToggleStudentDocumentStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetScopedByIdAsync(request.StudentDocumentId, tenantId, branchId, cancellationToken)
                ?? throw new KeyNotFoundException("Student document not found.");

            entity.IsActive = request.IsActive;
            entity.Remarks = string.IsNullOrWhiteSpace(request.Remarks) ? null : request.Remarks.Trim();
            entity.Status = request.IsActive
                ? entity.IsVerified
                    ? StudentDocumentStatuses.Verified
                    : StudentDocumentStatuses.Uploaded
                : StudentDocumentStatuses.Inactive;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = actor;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.ToResponse();
        }
    }
}