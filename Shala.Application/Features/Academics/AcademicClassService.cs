using Shala.Application.Common;
using Shala.Application.Repositories.Academics;
using Shala.Domain.Entities.Academics;
using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Academics;

public class AcademicClassService : IAcademicClassService
{
    private readonly IAcademicClassRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AcademicClassService(
        IAcademicClassRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<AcademicClassListItemResponse>>> GetAllAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(tenantId, cancellationToken);

        var result = items
            .OrderBy(x => x.Sequence)
            .ThenBy(x => x.Name)
            .Select(x => new AcademicClassListItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Sequence = x.Sequence,
                IsActive = x.IsActive
            })
            .ToList();

        return ApiResponse<List<AcademicClassListItemResponse>>.Ok(result);
    }

    public async Task<ApiResponse<AcademicClassListItemResponse>> GetByIdAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, tenantId, cancellationToken);

        if (entity is null)
            return ApiResponse<AcademicClassListItemResponse>.Fail("Class not found.");

        return ApiResponse<AcademicClassListItemResponse>.Ok(new AcademicClassListItemResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Sequence = entity.Sequence,
            IsActive = entity.IsActive
        });
    }

    public async Task<ApiResponse<int>> CreateAsync(
        int tenantId,
        CreateAcademicClassRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return ApiResponse<int>.Fail("Class name is required.");

        var exists = await _repository.ExistsByNameAsync(
            tenantId,
            request.Name.Trim(),
            null,
            cancellationToken);

        if (exists)
            return ApiResponse<int>.Fail("Class already exists.");

        var entity = new AcademicClass
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim(),
            Sequence = request.Sequence,
            IsActive = request.IsActive
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(entity.Id, "Class created successfully.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(
        int tenantId,
        string actor,
        UpdateAcademicClassRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(request.Id, tenantId, cancellationToken);

        if (entity is null)
            return ApiResponse<bool>.Fail("Class not found.");

        if (string.IsNullOrWhiteSpace(request.Name))
            return ApiResponse<bool>.Fail("Class name is required.");

        var exists = await _repository.ExistsByNameAsync(
            tenantId,
            request.Name.Trim(),
            request.Id,
            cancellationToken);

        if (exists)
            return ApiResponse<bool>.Fail("Class already exists.");

        entity.Name = request.Name.Trim();
        entity.Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim();
        entity.Sequence = request.Sequence;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Class updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, tenantId, cancellationToken);

        if (entity is null)
            return ApiResponse<bool>.Fail("Class not found.");

        _repository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Class deleted successfully.");
    }

    public async Task<ApiResponse<List<LookupItemResponse>>> GetLookupAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(tenantId, cancellationToken);

        var result = items
            .Where(x => x.IsActive)
            .OrderBy(x => x.Sequence)
            .ThenBy(x => x.Name)
            .Select(x => new LookupItemResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToList();

        return ApiResponse<List<LookupItemResponse>>.Ok(result);
    }
}