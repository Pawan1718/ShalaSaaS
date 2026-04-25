using Shala.Application.Common;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument;

public sealed class DocumentModelService : IDocumentModelService
{
    private readonly IDocumentModelRepository _repo;
    private readonly IUnitOfWork _uow;

    public DocumentModelService(
        IDocumentModelRepository repo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<List<DocumentModelResponse>> GetAllAsync(
     int tenantId,
     int branchId,
     CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetActiveAsync(tenantId, branchId, cancellationToken);

        return items
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(Map)
            .ToList();
    }

    public async Task<List<DocumentModelResponse>> GetActiveAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetActiveAsync(tenantId, branchId, cancellationToken);

        return items.Select(Map).ToList();
    }

    public async Task<DocumentModelResponse> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        CreateDocumentModelRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = new DocumentModel
        {
            TenantId = tenantId,
            BranchId = branchId,
            Name = request.Name.Trim(),
            Code = request.Code?.Trim() ?? string.Empty,
            Description = request.Description?.Trim(),
            IsRequired = request.IsRequired,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = actor
        };

        await _repo.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<DocumentModelResponse> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        UpdateDocumentModelRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null ||
            entity.TenantId != tenantId ||
            entity.BranchId != branchId)
            throw new Exception("Document not found.");

        entity.Name = request.Name.Trim();
        entity.Code = request.Code?.Trim() ?? string.Empty;
        entity.Description = request.Description?.Trim();
        entity.IsRequired = request.IsRequired;
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsActive = request.IsActive;

        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor;

        _repo.Update(entity);
        await _uow.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    private static DocumentModelResponse Map(DocumentModel x)
    {
        return new DocumentModelResponse
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Description = x.Description,
            IsRequired = x.IsRequired,
            DisplayOrder = x.DisplayOrder,
            IsActive = x.IsActive
        };
    }
}