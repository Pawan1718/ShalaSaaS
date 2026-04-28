using Shala.Application.Common;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Students;
using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.TenantConfig;

public class RollNumberSettingService : IRollNumberSettingService
{
    private readonly IRollNumberSettingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RollNumberSettingService(
        IRollNumberSettingRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<RollNumberSettingResponse>> GetAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByTenantIdAsync(tenantId, cancellationToken);

        if (entity is null)
        {
            return ApiResponse<RollNumberSettingResponse>.Ok(new RollNumberSettingResponse
            {
                TenantId = tenantId,
                AutoGenerate = true,
                AllowManualOverride = false,
                ResetPerAcademicYear = true,
                ResetPerClass = true,
                ResetPerSection = false,
                StartFrom = 1,
                NumberPadding = 3,
                Prefix = string.Empty,
                Format = "{number}"
            });
        }

        return ApiResponse<RollNumberSettingResponse>.Ok(new RollNumberSettingResponse
        {
            TenantId = entity.TenantId,
            AutoGenerate = entity.AutoGenerate,
            AllowManualOverride = entity.AllowManualOverride,
            ResetPerAcademicYear = entity.ResetPerAcademicYear,
            ResetPerClass = entity.ResetPerClass,
            ResetPerSection = entity.ResetPerSection,
            StartFrom = entity.StartFrom,
            NumberPadding = entity.NumberPadding,
            Prefix = entity.Prefix,
            Format = entity.Format
        });
    }

    public async Task<ApiResponse<bool>> SaveAsync(
        int tenantId,
        string actor,
        SaveRollNumberSettingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.StartFrom < 1)
            return ApiResponse<bool>.Fail("Start from must be at least 1.");

        if (request.NumberPadding < 1 || request.NumberPadding > 10)
            return ApiResponse<bool>.Fail("Number padding must be between 1 and 10.");

        if (string.IsNullOrWhiteSpace(request.Format))
            return ApiResponse<bool>.Fail("Format is required.");

        if (!request.Format.Contains("{number}", StringComparison.OrdinalIgnoreCase))
            return ApiResponse<bool>.Fail("Format must contain {number} placeholder.");

        var entity = await _repository.GetByTenantIdAsync(tenantId, cancellationToken);

        if (entity is null)
        {
            entity = new RollNumberSetting
            {
                TenantId = tenantId,
                AutoGenerate = request.AutoGenerate,
                AllowManualOverride = request.AllowManualOverride,
                ResetPerAcademicYear = request.ResetPerAcademicYear,
                ResetPerClass = request.ResetPerClass,
                ResetPerSection = request.ResetPerSection,
                StartFrom = request.StartFrom,
                NumberPadding = request.NumberPadding,
                Prefix = request.Prefix?.Trim() ?? string.Empty,
                Format = request.Format.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = actor
            };

            await _repository.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.AutoGenerate = request.AutoGenerate;
            entity.AllowManualOverride = request.AllowManualOverride;
            entity.ResetPerAcademicYear = request.ResetPerAcademicYear;
            entity.ResetPerClass = request.ResetPerClass;
            entity.ResetPerSection = request.ResetPerSection;
            entity.StartFrom = request.StartFrom;
            entity.NumberPadding = request.NumberPadding;
            entity.Prefix = request.Prefix?.Trim() ?? string.Empty;
            entity.Format = request.Format.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = actor;

            _repository.Update(entity);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Roll number settings saved successfully.");
    }
}