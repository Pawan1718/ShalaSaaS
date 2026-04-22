using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Web.Repositories.ConfigRepo
{
    public interface IRollNumberSettingRepository
    {
        Task<ApiResponse<RollNumberSettingResponse>?> GetAsync(int tenantId);
        Task<ApiResponse<bool>?> SaveAsync(SaveRollNumberSettingRequest request);
    }
}
