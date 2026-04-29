using Shala.Shared.Requests.Tenant;

namespace Shala.Application.Features.Identity
{
    public interface IUserService
    {
        Task<(bool Success, object Data)> CreateUserAsync(
            int tenantId,
            int actorBranchId,
            string? actorRole,
            CreateTenantUserRequest req);

        Task<(bool Success, object Data)> GetUsersAsync(int tenantId);
    }
}
