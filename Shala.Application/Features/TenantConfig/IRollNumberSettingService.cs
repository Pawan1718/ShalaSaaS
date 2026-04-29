using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.TenantConfig;

public interface IRollNumberSettingService
{
    Task<ApiResponse<RollNumberSettingResponse>> GetAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> SaveAsync(
        int tenantId,
        string actor,
        SaveRollNumberSettingRequest request,
        CancellationToken cancellationToken = default);
}