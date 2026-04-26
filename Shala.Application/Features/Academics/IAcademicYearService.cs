using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.Students;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.Academics;

public interface IAcademicYearService
{
    Task<ApiResponse<List<AcademicYearListItemResponse>>> GetAllAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<AcademicYearListItemResponse>> GetByIdAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<int>> CreateAsync(
        int tenantId,
        CreateAcademicYearRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> UpdateAsync(
        int tenantId,
        string actor,
        UpdateAcademicYearRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> SetCurrentAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> EnsureNextAcademicYearAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<AcademicYearSettingResponse>> GetSettingsAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> SaveSettingsAsync(
        int tenantId,
        string actor,
        SaveAcademicYearSettingRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<List<LookupItemResponse>>> GetLookupAsync(
        int tenantId,
        CancellationToken cancellationToken = default);
}