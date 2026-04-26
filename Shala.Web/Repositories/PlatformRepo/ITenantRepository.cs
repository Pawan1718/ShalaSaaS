using Shala.Shared.Common;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.PlatformRepo
{
    public interface ITenantRepository
    {
        Task<ServerResponseHelper<List<TenantListItemResponse>>> GetTenantsAsync();
        Task<ServerResponseHelper<List<TenantUserResponse>>> GetTenantUsersAsync(int tenantId);
        Task<ServerResponseHelper<PagedResult<TenantListItemResponse>>> GetTenantsPagedAsync(TenantListRequest request);
        Task<ServerResponseHelper<TenantDetailResponse>> GetTenantByIdAsync(int tenantId);
        Task<ServerResponseHelper<CreateTenantResponse>> CreateTenantAsync(CreateTenantRequest request); Task<ServerResponseHelper<object>> ResetTenantAdminPasswordAsync(int tenantId, ResetTenantAdminPasswordRequest request);
        Task<ServerResponseHelper<object>> UpdateTenantAdminEmailAsync(int tenantId, UpdateTenantAdminEmailRequest request);
        Task<ServerResponseHelper<object>> UpdateTenantStatusAsync(int tenantId, UpdateTenantStatusRequest request);
        Task<ServerResponseHelper<object>> UpdateTenantBasicInfoAsync(int tenantId, UpdateTenantBasicInfoRequest request);

    }
}