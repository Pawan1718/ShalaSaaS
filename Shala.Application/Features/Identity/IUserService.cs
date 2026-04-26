using Shala.Shared.Requests.Platform;
using Shala.Shared.Requests.Tenant;

namespace Shala.Application.Features.Identity
{
    public interface IUserService
    {
        Task<(bool Success, object Data)> CreateUserAsync(int tenantId, CreateTenantUserRequest req);
        Task<(bool Success, object Data)> GetUsersAsync(int tenantId);
    }
}