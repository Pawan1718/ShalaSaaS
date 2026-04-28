using Shala.Shared.Requests.Platform;

namespace Shala.Application.Features.Platform;

public interface ITenantProvisionService
{
    Task<(bool Success, object Data)> CreateTenantAsync(CreateTenantRequest req);
    Task<(bool Success, object Data)> GetTenantsAsync();
    Task<(bool Success, object Data)> GetTenantsPagedAsync(TenantListRequest req);
    Task<(bool Success, object Data)> UpdateTenantBasicInfoAsync(int tenantId, UpdateTenantBasicInfoRequest req);
    Task<(bool Success, object Data)> GetTenantByIdAsync(int tenantId);
    Task<(bool Success, object Data)> ResetTenantAdminPasswordAsync(int tenantId, ResetTenantAdminPasswordRequest req);
    Task<(bool Success, object Data)> UpdateTenantAdminEmailAsync(int tenantId, UpdateTenantAdminEmailRequest req);
    Task<(bool Success, object Data)> UpdateTenantStatusAsync(int tenantId, UpdateTenantStatusRequest req);
}