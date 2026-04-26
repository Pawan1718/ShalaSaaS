using Shala.Application.Common;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Application.Repositories.Students;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument;

public sealed class StudentDocumentChecklistService : IStudentDocumentChecklistService
{
    private readonly IDocumentModelRepository _documentModelRepository;
    private readonly IStudentDocumentChecklistRepository _checklistRepository;
    private readonly IStudentAdmissionRepository _admissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentDocumentChecklistService(
        IDocumentModelRepository documentModelRepository,
        IStudentDocumentChecklistRepository checklistRepository,
        IStudentAdmissionRepository admissionRepository,
        IUnitOfWork unitOfWork)
    {
        _documentModelRepository = documentModelRepository;
        _checklistRepository = checklistRepository;
        _admissionRepository = admissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentDocumentChecklistResponse> GetByAdmissionAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default)
    {
        await EnsureAdmissionExistsAsync(tenantId, branchId, studentAdmissionId, cancellationToken);

        var documentModels = await _documentModelRepository.GetActiveAsync(
            tenantId,
            branchId,
            cancellationToken);

        var checklistItems = await _checklistRepository.GetByAdmissionAsync(
            tenantId,
            branchId,
            studentAdmissionId,
            cancellationToken);

        var checklistLookup = checklistItems.ToDictionary(x => x.DocumentModelId);

        var items = documentModels
            .Select(model =>
            {
                checklistLookup.TryGetValue(model.Id, out var checklist);
                return model.ToChecklistItemResponse(checklist);
            })
            .OrderByDescending(x => x.IsRequired)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.DocumentName)
            .ToList();

        return BuildResponse(studentAdmissionId, items);
    }

    public async Task<StudentDocumentChecklistResponse> SaveAsync(
        int tenantId,
        int branchId,
        string actor,
        SaveStudentDocumentChecklistRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.StudentAdmissionId <= 0)
            throw new InvalidOperationException("Student admission is required.");

        await EnsureAdmissionExistsAsync(
            tenantId,
            branchId,
            request.StudentAdmissionId,
            cancellationToken);

        var activeModels = await _documentModelRepository.GetActiveAsync(
            tenantId,
            branchId,
            cancellationToken);

        var activeModelIds = activeModels.Select(x => x.Id).ToHashSet();

        foreach (var item in request.Items)
        {
            if (!activeModelIds.Contains(item.DocumentModelId))
                throw new InvalidOperationException("Invalid or inactive document selected.");
        }

        var existingItems = await _checklistRepository.GetByAdmissionAsync(
            tenantId,
            branchId,
            request.StudentAdmissionId,
            cancellationToken);

        var existingLookup = existingItems.ToDictionary(x => x.DocumentModelId);

        foreach (var item in request.Items)
        {
            existingLookup.TryGetValue(item.DocumentModelId, out var entity);

            var isNew = entity is null;

            if (isNew)
            {
                entity = new StudentDocumentChecklist
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    StudentAdmissionId = request.StudentAdmissionId,
                    DocumentModelId = item.DocumentModelId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = actor
                };

                await _checklistRepository.AddAsync(entity, cancellationToken);
            }

            entity.IsReceived = item.IsReceived;
            entity.Remark = string.IsNullOrWhiteSpace(item.Remark)
                ? null
                : item.Remark.Trim();

            entity.ReceivedDate = item.IsReceived
                ? entity.ReceivedDate ?? DateTime.UtcNow
                : null;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = actor;

            if (!isNew)
                _checklistRepository.Update(entity);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByAdmissionAsync(
            tenantId,
            branchId,
            request.StudentAdmissionId,
            cancellationToken);
    }

    public Task<StudentDocumentChecklistResponse> ValidateAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default)
    {
        return GetByAdmissionAsync(tenantId, branchId, studentAdmissionId, cancellationToken);
    }

    private async Task EnsureAdmissionExistsAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken)
    {
        var admission = await _admissionRepository.GetByIdAsync(
            studentAdmissionId,
            tenantId,
            branchId,
            cancellationToken);

        if (admission is null)
            throw new KeyNotFoundException("Student admission not found.");
    }

    private static StudentDocumentChecklistResponse BuildResponse(
        int studentAdmissionId,
        List<StudentDocumentChecklistItemResponse> items)
    {
        var pendingRequired = items.Count(x => x.IsRequired && !x.IsReceived);

        return new StudentDocumentChecklistResponse
        {
            StudentAdmissionId = studentAdmissionId,
            TotalDocuments = items.Count,
            RequiredDocuments = items.Count(x => x.IsRequired),
            ReceivedDocuments = items.Count(x => x.IsReceived),
            PendingRequiredDocuments = pendingRequired,
            IsValid = pendingRequired == 0,
            Items = items
        };
    }
}