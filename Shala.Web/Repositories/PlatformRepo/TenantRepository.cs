using Shala.Shared.Common;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.PlatformRepo
{
    public class TenantRepository : ITenantRepository
    {
        private readonly IHttpService _httpService;

        public TenantRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ServerResponseHelper<List<TenantListItemResponse>>> GetTenantsAsync()
        {
            return await _httpService.GetAsync<List<TenantListItemResponse>>("api/platform/tenants");
        }

        public async Task<ServerResponseHelper<PagedResult<TenantListItemResponse>>> GetTenantsPagedAsync(TenantListRequest request)
        {
            return await _httpService.PostAsync<TenantListRequest, PagedResult<TenantListItemResponse>>(
                "api/platform/tenants/paged",
                request);
        }

        public async Task<ServerResponseHelper<List<TenantUserResponse>>> GetTenantUsersAsync(int tenantId)
        {
            return await _httpService.GetAsync<List<TenantUserResponse>>($"api/platform/tenants/{tenantId}/users");
        }

        public async Task<ServerResponseHelper<TenantDetailResponse>> GetTenantByIdAsync(int tenantId)
        {
            return await _httpService.GetAsync<TenantDetailResponse>($"api/platform/tenants/{tenantId}");
        }

        public async Task<ServerResponseHelper<CreateTenantResponse>> CreateTenantAsync(CreateTenantRequest request)
        {
            return await _httpService.PostAsync<CreateTenantRequest, CreateTenantResponse>(
                "api/platform/tenants",
                request);
        }

        public async Task<ServerResponseHelper<object>> ResetTenantAdminPasswordAsync(int tenantId, ResetTenantAdminPasswordRequest request)
        {
            return await _httpService.PutAsync<ResetTenantAdminPasswordRequest, object>(
                $"api/platform/tenants/{tenantId}/admin/reset-password", request);
        }

        public async Task<ServerResponseHelper<object>> UpdateTenantAdminEmailAsync(int tenantId, UpdateTenantAdminEmailRequest request)
        {
            return await _httpService.PutAsync<UpdateTenantAdminEmailRequest, object>(
                $"api/platform/tenants/{tenantId}/admin/update-email", request);
        }

        public async Task<ServerResponseHelper<object>> UpdateTenantStatusAsync(int tenantId, UpdateTenantStatusRequest request)
        {
            return await _httpService.PutAsync<UpdateTenantStatusRequest, object>(
                $"api/platform/tenants/{tenantId}/status", request);
        }

        public async Task<ServerResponseHelper<object>> UpdateTenantBasicInfoAsync(int tenantId, UpdateTenantBasicInfoRequest request)
        {
            return await _httpService.PutAsync<UpdateTenantBasicInfoRequest, object>(
                $"api/platform/tenants/{tenantId}/basic-info", request);
        }
    }
}