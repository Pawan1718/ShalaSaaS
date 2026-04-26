using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Common;

namespace Shala.Application.Features.Fees;

public class FeeHeadService : IFeeHeadService
{
    private readonly IFeeHeadRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public FeeHeadService(
        IFeeHeadRepository repo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public Task<List<FeeHead>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetAllAsync(tenantId, branchId, cancellationToken);
    }

    public Task<FeeHead?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
    }

    public async Task<(bool Success, string Message, FeeHead? Data)> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        FeeHead entity,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Name))
            return (false, "Fee head name is required.", null);

        if (string.IsNullOrWhiteSpace(entity.Code))
            return (false, "Fee head code is required.", null);

        entity.TenantId = tenantId;
        entity.BranchId = branchId;
        entity.Name = entity.Name.Trim();
        entity.Code = entity.Code.Trim().ToUpperInvariant();
        entity.Description = string.IsNullOrWhiteSpace(entity.Description)
            ? null
            : entity.Description.Trim();
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = actor;

        var existing = await _repo.GetByCodeAsync(
            entity.Code,
            tenantId,
            branchId,
            cancellationToken);

        if (existing is not null)
            return (false, "Fee head code already exists.", null);

        await _repo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Fee head created successfully.", entity);
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        FeeHead entity,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Name))
            return (false, "Fee head name is required.");

        if (string.IsNullOrWhiteSpace(entity.Code))
            return (false, "Fee head code is required.");

        var existing = await _repo.GetByIdAsync(entity.Id, tenantId, branchId, cancellationToken);

        if (existing is null)
            return (false, "Fee head not found.");

        var normalizedCode = entity.Code.Trim().ToUpperInvariant();

        var duplicateCode = await _repo.GetByCodeAsync(
            normalizedCode,
            tenantId,
            branchId,
            cancellationToken);

        if (duplicateCode is not null && duplicateCode.Id != entity.Id)
            return (false, "Fee head code already exists.");

        existing.Name = entity.Name.Trim();
        existing.Code = normalizedCode;
        existing.Description = string.IsNullOrWhiteSpace(entity.Description)
            ? null
            : entity.Description.Trim();
        existing.IsActive = entity.IsActive;
        existing.IsRegistrationFee = entity.IsRegistrationFee;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = actor;

        _repo.Update(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Fee head updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);

        if (existing is null)
            return (false, "Fee head not found.");

        var isInUse = await _repo.IsInUseAsync(id, tenantId, branchId, cancellationToken);
        if (isInUse)
            return (false, "Fee head is already in use and cannot be deleted.");

        _repo.Delete(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Fee head deleted successfully.");
    }

    public Task<PagedResult<FeeHead>> GetPagedAsync(
        int tenantId,
        int branchId,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetPagedAsync(tenantId, branchId, request, cancellationToken);
    }
}