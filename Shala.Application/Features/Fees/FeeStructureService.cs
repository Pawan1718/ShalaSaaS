using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;

namespace Shala.Application.Features.Fees;

public class FeeStructureService : IFeeStructureService
{
    private readonly IFeeStructureRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public FeeStructureService(
        IFeeStructureRepository repo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public Task<List<FeeStructure>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetAllAsync(tenantId, branchId, cancellationToken);
    }

    public Task<FeeStructure?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
    }

    public Task<FeeStructure?> GetWithItemsAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetWithItemsAsync(id, tenantId, branchId, cancellationToken);
    }

    public async Task<(bool Success, string Message, FeeStructure? Data)> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        FeeStructure entity,
        CancellationToken cancellationToken = default)
    {
        entity.TenantId = tenantId;
        entity.BranchId = branchId;

        var validation = ValidateStructure(entity);
        if (!validation.Success)
            return (false, validation.Message, null);

        entity.Name = entity.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(entity.Description) ? null : entity.Description.Trim();
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = actor;

        NormalizeItems(entity.Items);

        foreach (var item in entity.Items)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.CreatedBy = actor;
        }

        await _repo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Fee structure created successfully.", entity);
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        FeeStructure entity,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetWithItemsAsync(entity.Id, tenantId, branchId, cancellationToken);

        if (existing is null)
            return (false, "Fee structure not found.");

        entity.TenantId = tenantId;
        entity.BranchId = branchId;

        var validation = ValidateStructure(entity);
        if (!validation.Success)
            return (false, validation.Message);

        existing.Name = entity.Name.Trim();
        existing.Description = string.IsNullOrWhiteSpace(entity.Description) ? null : entity.Description.Trim();
        existing.AcademicYearId = entity.AcademicYearId;
        existing.AcademicClassId = entity.AcademicClassId;
        existing.IsActive = entity.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = actor;

        existing.Items.Clear();

        foreach (var item in entity.Items)
        {
            existing.Items.Add(new FeeStructureItem
            {
                FeeHeadId = item.FeeHeadId,
                Label = item.Label.Trim(),
                Amount = item.Amount,
                FrequencyType = item.FrequencyType,
                StartMonth = item.StartMonth,
                EndMonth = item.EndMonth,
                DueDay = item.DueDay,
                ApplyType = item.ApplyType,
                IsOptional = item.IsOptional,
                IsActive = item.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = actor
            });
        }

        _repo.Update(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Fee structure updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);

        if (existing is null)
            return (false, "Fee structure not found.");

        _repo.Delete(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Fee structure deleted successfully.");
    }

    private static (bool Success, string Message) ValidateStructure(FeeStructure entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Name))
            return (false, "Fee structure name is required.");

        if (entity.AcademicYearId <= 0)
            return (false, "Academic year is required.");

        if (entity.AcademicClassId <= 0)
            return (false, "Academic class is required.");

        if (entity.Items is null || entity.Items.Count == 0)
            return (false, "At least one fee structure item is required.");

        foreach (var item in entity.Items)
        {
            if (item.FeeHeadId <= 0)
                return (false, "Fee head is required.");

            if (string.IsNullOrWhiteSpace(item.Label))
                return (false, "Fee structure item label is required.");

            if (item.Amount <= 0)
                return (false, "Amount must be greater than zero.");
        }

        return (true, string.Empty);
    }

    private static void NormalizeItems(IEnumerable<FeeStructureItem> items)
    {
        foreach (var item in items)
            item.Label = item.Label.Trim();
    }
}