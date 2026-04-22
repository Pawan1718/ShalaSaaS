using Shala.Application.Common;
using Shala.Application.Repositories.Platform;
using Shala.Domain.Entities.Platform;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;

namespace Shala.Application.Features.Platform;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBranchCodeGenerator _branchCodeGenerator;

    public BranchService(
        IBranchRepository repository,
        IUnitOfWork unitOfWork,
        IBranchCodeGenerator branchCodeGenerator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _branchCodeGenerator = branchCodeGenerator;
    }

    public async Task<(bool Success, BranchResponse? Data, string? Message)> CreateAsync(
        CreateBranchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.TenantId <= 0)
            return (false, null, "Invalid tenant.");

        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, null, "Branch name is required.");

        var existingBranches = await _repository.GetAllAsync(request.TenantId, cancellationToken);

        if (request.IsMainBranch && existingBranches.Any(x => x.IsMainBranch))
            return (false, null, "Main branch already exists for this tenant.");

        var generatedCode = await _branchCodeGenerator.GenerateAsync(
            request.TenantId,
            request.Name,
            cancellationToken);

        var entity = new Branch
        {
            TenantId = request.TenantId,
            Name = request.Name.Trim(),
            Code = generatedCode,
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            AddressLine1 = string.IsNullOrWhiteSpace(request.AddressLine1) ? null : request.AddressLine1.Trim(),
            AddressLine2 = string.IsNullOrWhiteSpace(request.AddressLine2) ? null : request.AddressLine2.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim(),
            Pincode = string.IsNullOrWhiteSpace(request.Pincode) ? null : request.Pincode.Trim(),
            PrincipalName = string.IsNullOrWhiteSpace(request.PrincipalName) ? null : request.PrincipalName.Trim(),
            IsMainBranch = request.IsMainBranch,
            IsActive = request.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, MapToResponse(entity), "Branch created successfully.");
    }

    public async Task<(bool Success, BranchResponse? Data, string? Message)> GetByIdAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0 || branchId <= 0)
            return (false, null, "Invalid branch request.");

        var entity = await _repository.GetByIdAsync(tenantId, branchId, cancellationToken);

        if (entity is null)
            return (false, null, "Branch not found.");

        return (true, MapToResponse(entity), "Branch details loaded successfully.");
    }

    public async Task<(bool Success, List<BranchResponse>? Data, string? Message)> GetAllAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0)
            return (false, null, "Invalid tenant.");

        var items = await _repository.GetAllAsync(tenantId, cancellationToken);

        return (true, items.Select(MapToResponse).ToList(), "Branches loaded successfully.");
    }

    public async Task<(bool Success, BranchResponse? Data, string? Message)> UpdateAsync(
        int tenantId,
        int branchId,
        UpdateBranchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0 || branchId <= 0)
            return (false, null, "Invalid branch request.");

        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, null, "Branch name is required.");

        if (string.IsNullOrWhiteSpace(request.Code))
            return (false, null, "Branch code is required.");

        var entity = await _repository.GetByIdAsync(tenantId, branchId, cancellationToken);

        if (entity is null)
            return (false, null, "Branch not found.");

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var exists = await _repository.ExistsByCodeAsync(
            tenantId,
            normalizedCode,
            branchId,
            cancellationToken);

        if (exists)
            return (false, null, "Branch code already exists.");

        if (request.IsMainBranch)
        {
            var branches = await _repository.GetAllAsync(tenantId, cancellationToken);
            var anotherMainBranchExists = branches.Any(x => x.IsMainBranch && x.Id != branchId);

            if (anotherMainBranchExists)
                return (false, null, "Main branch already exists for this tenant.");
        }

        entity.Name = request.Name.Trim();
        entity.Code = normalizedCode;
        entity.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        entity.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        entity.AddressLine1 = string.IsNullOrWhiteSpace(request.AddressLine1) ? null : request.AddressLine1.Trim();
        entity.AddressLine2 = string.IsNullOrWhiteSpace(request.AddressLine2) ? null : request.AddressLine2.Trim();
        entity.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        entity.State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim();
        entity.Pincode = string.IsNullOrWhiteSpace(request.Pincode) ? null : request.Pincode.Trim();
        entity.PrincipalName = string.IsNullOrWhiteSpace(request.PrincipalName) ? null : request.PrincipalName.Trim();
        entity.IsMainBranch = request.IsMainBranch;
        entity.IsActive = request.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, MapToResponse(entity), "Branch updated successfully.");
    }

    public async Task<(bool Success, bool Data, string? Message)> DeleteAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0 || branchId <= 0)
            return (false, false, "Invalid branch request.");

        var entity = await _repository.GetByIdAsync(tenantId, branchId, cancellationToken);

        if (entity is null)
            return (false, false, "Branch not found.");

        var branches = await _repository.GetAllAsync(tenantId, cancellationToken);

        if (branches.Count <= 1)
            return (false, false, "At least one branch must exist. The only branch cannot be deleted.");

        if (entity.IsMainBranch)
            return (false, false, "Main branch cannot be deleted.");

        _repository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, true, "Branch deleted successfully.");
    }

    private static BranchResponse MapToResponse(Branch entity)
    {
        return new BranchResponse
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Code = entity.Code,
            Email = entity.Email,
            Phone = entity.Phone,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            City = entity.City,
            State = entity.State,
            Pincode = entity.Pincode,
            PrincipalName = entity.PrincipalName,
            IsMainBranch = entity.IsMainBranch,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}