using Shala.Application.Common;
using Shala.Application.Repositories.Academics;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Academics;
using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.Students;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.Academics;

public class AcademicYearService : IAcademicYearService
{
    private readonly IAcademicYearRepository _repository;
    private readonly IAcademicYearSettingRepository _settingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcademicYearService(
        IAcademicYearRepository repository,
        IAcademicYearSettingRepository settingRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _settingRepository = settingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<AcademicYearListItemResponse>>> GetAllAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        await EnsureNextAcademicYearAsync(tenantId, cancellationToken);

        var items = await _repository.GetAllAsync(tenantId, cancellationToken);

        var result = items
            .OrderByDescending(x => x.StartDate)
            .Select(x => new AcademicYearListItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsCurrent = x.IsCurrent,
                IsActive = x.IsActive
            })
            .ToList();

        return ApiResponse<List<AcademicYearListItemResponse>>.Ok(result);
    }

    public async Task<ApiResponse<AcademicYearListItemResponse>> GetByIdAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, tenantId, cancellationToken);

        if (entity is null)
            return ApiResponse<AcademicYearListItemResponse>.Fail("Academic year not found.");

        return ApiResponse<AcademicYearListItemResponse>.Ok(new AcademicYearListItemResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsCurrent = entity.IsCurrent,
            IsActive = entity.IsActive
        });
    }

    public async Task<ApiResponse<int>> CreateAsync(
        int tenantId,
        CreateAcademicYearRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.StartDate >= request.EndDate)
            return ApiResponse<int>.Fail("Start date must be before end date.");

        var overlapExists = await _repository.HasDateOverlapAsync(
            tenantId,
            request.StartDate,
            request.EndDate,
            null,
            cancellationToken);

        if (overlapExists)
            return ApiResponse<int>.Fail("Academic year date range overlaps with an existing academic year.");

        var entity = new AcademicYear
        {
            TenantId = tenantId,
            Name = GenerateName(request.StartDate, request.EndDate),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent,
            IsActive = request.IsActive
        };

        if (entity.IsCurrent)
        {
            await _repository.ResetCurrentAsync(tenantId, null, cancellationToken);
        }

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(entity.Id, "Academic year created successfully.");
    }

    public async Task<ApiResponse<bool>> UpdateAsync(
        int tenantId,
        string actor,
        UpdateAcademicYearRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(request.Id, tenantId, cancellationToken);

        if (entity is null)
            return ApiResponse<bool>.Fail("Academic year not found.");

        if (request.StartDate >= request.EndDate)
            return ApiResponse<bool>.Fail("Start date must be before end date.");

        var overlapExists = await _repository.HasDateOverlapAsync(
            tenantId,
            request.StartDate,
            request.EndDate,
            request.Id,
            cancellationToken);

        if (overlapExists)
            return ApiResponse<bool>.Fail("Academic year date range overlaps with an existing academic year.");

        if (request.IsCurrent)
        {
            await _repository.ResetCurrentAsync(tenantId, request.Id, cancellationToken);
        }

        entity.Name = GenerateName(request.StartDate, request.EndDate);
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.IsCurrent = request.IsCurrent;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Academic year updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, tenantId, cancellationToken);

        if (entity is null)
            return ApiResponse<bool>.Fail("Academic year not found.");

        var hasAdmissions = await _repository.HasAdmissionsAsync(id, tenantId, cancellationToken);
        if (hasAdmissions)
            return ApiResponse<bool>.Fail("Academic year cannot be deleted because admissions exist.");

        _repository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Academic year deleted successfully.");
    }

    public async Task<ApiResponse<bool>> SetCurrentAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var target = await _repository.GetByIdAsync(id, tenantId, cancellationToken);

        if (target is null)
            return ApiResponse<bool>.Fail("Academic year not found.");

        await _repository.ResetCurrentAsync(tenantId, id, cancellationToken);

        target.IsCurrent = true;
        _repository.Update(target);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.Ok(true, "Current academic year updated successfully.");
    }

    public async Task<ApiResponse<bool>> EnsureNextAcademicYearAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var setting = await _settingRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        if (setting == null || !setting.AutoCreateNextYear)
            return ApiResponse<bool>.Ok(true);

        var items = await _repository.GetAllAsync(tenantId, cancellationToken);
        var latest = items.OrderByDescending(x => x.EndDate).FirstOrDefault();

        if (latest == null)
            return ApiResponse<bool>.Ok(true);

        var triggerDate = latest.EndDate.AddDays(-setting.CreateBeforeDays);
        if (DateTime.UtcNow.Date < triggerDate.Date)
            return ApiResponse<bool>.Ok(true);

        var nextStart = latest.EndDate.AddDays(1);
        var nextEnd = nextStart.AddYears(1).AddDays(-1);
        var nextName = GenerateName(nextStart, nextEnd);

        var exists = items.Any(x => x.Name == nextName);
        if (exists)
            return ApiResponse<bool>.Ok(true);

        await _repository.AddAsync(new AcademicYear
        {
            TenantId = tenantId,
            Name = nextName,
            StartDate = nextStart,
            EndDate = nextEnd,
            IsCurrent = false,
            IsActive = true
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<AcademicYearSettingResponse>> GetSettingsAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _settingRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        if (entity == null)
        {
            return ApiResponse<AcademicYearSettingResponse>.Ok(new AcademicYearSettingResponse
            {
                TenantId = tenantId,
                StartMonth = 4,
                StartDay = 1,
                EndMonth = 3,
                EndDay = 31,
                AutoCreateNextYear = true,
                CreateBeforeDays = 30
            });
        }

        return ApiResponse<AcademicYearSettingResponse>.Ok(new AcademicYearSettingResponse
        {
            TenantId = entity.TenantId,
            StartMonth = entity.StartMonth,
            StartDay = entity.StartDay,
            EndMonth = entity.EndMonth,
            EndDay = entity.EndDay,
            AutoCreateNextYear = entity.AutoCreateNextYear,
            CreateBeforeDays = entity.CreateBeforeDays
        });
    }

    public async Task<ApiResponse<bool>> SaveSettingsAsync(
        int tenantId,
        string actor,
        SaveAcademicYearSettingRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _settingRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        if (entity == null)
        {
            entity = new AcademicYearSetting
            {
                TenantId = tenantId
            };

            await _settingRepository.AddAsync(entity, cancellationToken);
        }

        entity.StartMonth = request.StartMonth;
        entity.StartDay = request.StartDay;
        entity.EndMonth = request.EndMonth;
        entity.EndDay = request.EndDay;
        entity.AutoCreateNextYear = request.AutoCreateNextYear;
        entity.CreateBeforeDays = request.CreateBeforeDays;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.Ok(true, "Academic year settings saved successfully.");
    }

    public async Task<ApiResponse<List<LookupItemResponse>>> GetLookupAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(tenantId, cancellationToken);

        var result = items
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.StartDate)
            .Select(x => new LookupItemResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToList();

        return ApiResponse<List<LookupItemResponse>>.Ok(result);
    }

    private static string GenerateName(DateTime startDate, DateTime endDate)
    {
        return $"{startDate:yyyy}-{endDate:yy}";
    }
}