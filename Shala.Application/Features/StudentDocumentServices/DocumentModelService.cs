using Shala.Application.Common;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument
{
    public sealed class DocumentModelService : IDocumentModelService
    {
        private readonly IDocumentModelRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentModelService(
            IDocumentModelRepository repo,
            IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DocumentModelResponse>> GetAllAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            var items = await _repo.GetWhereAsync(
                x => x.TenantId == tenantId && x.BranchId == branchId,
                query => query,
                orderBy: q => q.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name),
                cancellationToken: cancellationToken);

            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task<List<DocumentModelResponse>> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            var items = await _repo.GetActiveAsync(tenantId, branchId, cancellationToken);
            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task<DocumentModelResponse?> GetByIdAsync(
            int tenantId,
            int branchId,
            int id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.FirstOrDefaultAsync(
                x => x.Id == id && x.TenantId == tenantId && x.BranchId == branchId,
                cancellationToken);

            return entity?.ToResponse();
        }

        public async Task<DocumentModelResponse> CreateAsync(
            int tenantId,
            int branchId,
            string actor,
            CreateDocumentModelRequest request,
            CancellationToken cancellationToken = default)
        {
            var normalizedCode = StudentDocumentValidation.NormalizeCode(request.Code);

            var existing = await _repo.GetByCodeAsync(
                normalizedCode,
                tenantId,
                branchId,
                cancellationToken);

            if (existing is not null)
                throw new InvalidOperationException("Document model code already exists.");

            var entity = new Domain.Entities.StudentDocuments.DocumentModel
            {
                TenantId = tenantId,
                BranchId = branchId,
                Name = StudentDocumentValidation.NormalizeRequired(request.Name, "Name"),
                Code = normalizedCode,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                IsRequired = request.IsRequired,
                IsAiValidationEnabled = request.IsAiValidationEnabled,
                BlockAdmissionOnMismatch = request.BlockAdmissionOnMismatch,
                IsActive = true,
                AllowedFileTypes = string.IsNullOrWhiteSpace(request.AllowedFileTypes) ? null : request.AllowedFileTypes.Trim(),
                MaxFileSizeInKb = request.MaxFileSizeInKb,
                RequiredFieldsJson = string.IsNullOrWhiteSpace(request.RequiredFieldsJson) ? null : request.RequiredFieldsJson.Trim(),
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = actor
            };

            await _repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.ToResponse();
        }

        public async Task<DocumentModelResponse> UpdateAsync(
            int tenantId,
            int branchId,
            string actor,
            UpdateDocumentModelRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.FirstOrDefaultAsync(
                x => x.Id == request.Id && x.TenantId == tenantId && x.BranchId == branchId,
                cancellationToken)
                ?? throw new KeyNotFoundException("Document model not found.");

            var normalizedCode = StudentDocumentValidation.NormalizeCode(request.Code);

            var duplicate = await _repo.FirstOrDefaultAsync(
                x => x.Id != request.Id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId &&
                     x.Code == normalizedCode,
                cancellationToken);

            if (duplicate is not null)
                throw new InvalidOperationException("Document model code already exists.");

            entity.Name = StudentDocumentValidation.NormalizeRequired(request.Name, "Name");
            entity.Code = normalizedCode;
            entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            entity.IsRequired = request.IsRequired;
            entity.IsAiValidationEnabled = request.IsAiValidationEnabled;
            entity.BlockAdmissionOnMismatch = request.BlockAdmissionOnMismatch;
            entity.AllowedFileTypes = string.IsNullOrWhiteSpace(request.AllowedFileTypes) ? null : request.AllowedFileTypes.Trim();
            entity.MaxFileSizeInKb = request.MaxFileSizeInKb;
            entity.RequiredFieldsJson = string.IsNullOrWhiteSpace(request.RequiredFieldsJson) ? null : request.RequiredFieldsJson.Trim();
            entity.DisplayOrder = request.DisplayOrder;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = actor;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.ToResponse();
        }

        public async Task<DocumentModelResponse> ToggleStatusAsync(
            int tenantId,
            int branchId,
            string actor,
            ToggleDocumentModelStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.FirstOrDefaultAsync(
                x => x.Id == request.Id && x.TenantId == tenantId && x.BranchId == branchId,
                cancellationToken)
                ?? throw new KeyNotFoundException("Document model not found.");

            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = actor;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.ToResponse();
        }
    }
}