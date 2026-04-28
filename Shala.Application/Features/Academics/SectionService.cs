using Shala.Application.Common;
using Shala.Application.Repositories.Academics;
using Shala.Domain.Entities.Academics;
using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Academics;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SectionService(
        ISectionRepository sectionRepository,
        IUnitOfWork unitOfWork)
    {
        _sectionRepository = sectionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<SectionListItemResponse>>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        var sections = await _sectionRepository.GetAllAsync(tenantId, branchId, cancellationToken);

        var result = sections
            .OrderBy(x => x.AcademicClass!.Name)
            .ThenBy(x => x.Name)
            .Select(x => new SectionListItemResponse
            {
                Id = x.Id,
                AcademicClassId = x.AcademicClassId,
                ClassName = x.AcademicClass?.Name ?? "-",
                Name = x.Name,
                Capacity = x.Capacity,
                IsActive = x.IsActive
            })
            .ToList();

        return ApiResponse<List<SectionListItemResponse>>.Ok(result, "Sections loaded successfully.");
    }

    public async Task<ApiResponse<SectionListItemResponse>> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var section = await _sectionRepository.GetByIdAsync(id, tenantId, branchId, cancellationToken);

        if (section is null)
            return ApiResponse<SectionListItemResponse>.Fail("Section not found.");

        return ApiResponse<SectionListItemResponse>.Ok(new SectionListItemResponse
        {
            Id = section.Id,
            AcademicClassId = section.AcademicClassId,
            ClassName = section.AcademicClass?.Name ?? "-",
            Name = section.Name,
            Capacity = section.Capacity,
            IsActive = section.IsActive
        }, "Section loaded successfully.");
    }

    public async Task<ApiResponse<int>> CreateAsync(
        int tenantId,
        int branchId,
        CreateSectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();

        var exists = await _sectionRepository.ExistsByNameAsync(
            tenantId,
            branchId,
            request.AcademicClassId,
            name,
            null,
            cancellationToken);

        if (exists)
            return ApiResponse<int>.Fail("Section name already exists for this class.");

        var entity = new Section
        {
            TenantId = tenantId,
            BranchId = branchId,
            AcademicClassId = request.AcademicClassId,
            Name = name,
            Capacity = request.Capacity,
            IsActive = request.IsActive
        };

        await _sectionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(entity.Id, "Section created successfully.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        UpdateSectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var section = await _sectionRepository.GetByIdAsync(request.Id, tenantId, branchId, cancellationToken);

        if (section is null)
            return ApiResponse<bool>.Fail("Section not found.");

        var name = request.Name.Trim();

        var exists = await _sectionRepository.ExistsByNameAsync(
            tenantId,
            branchId,
            request.AcademicClassId,
            name,
            request.Id,
            cancellationToken);

        if (exists)
            return ApiResponse<bool>.Fail("Section name already exists for this class.");

        section.AcademicClassId = request.AcademicClassId;
        section.Name = name;
        section.Capacity = request.Capacity;
        section.IsActive = request.IsActive;
        section.UpdatedAt = DateTime.UtcNow;
        section.UpdatedBy = actor;

        _sectionRepository.Update(section);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Section updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var section = await _sectionRepository.GetByIdAsync(id, tenantId, branchId, cancellationToken);

        if (section is null)
            return ApiResponse<bool>.Fail("Section not found.");

        _sectionRepository.Delete(section);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Section deleted successfully.");
    }

    public async Task<ApiResponse<List<LookupItemResponse>>> GetLookupByClassAsync(
        int tenantId,
        int branchId,
        int classId,
        CancellationToken cancellationToken = default)
    {
        var sections = await _sectionRepository.GetByClassAsync(tenantId, branchId, classId, cancellationToken);

        var result = sections
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new LookupItemResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToList();

        return ApiResponse<List<LookupItemResponse>>.Ok(result, "Section lookup loaded successfully.");
    }
}