using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Platform;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.TenantRepo;

public interface ITenantUserRepository
{
    Task<ServerResponseHelper<List<UserListItemResponse>>> GetUsersAsync(
        CancellationToken cancellationToken = default);

    Task<ServerResponseHelper<object>> CreateUserAsync(
        CreateTenantUserRequest request,
        CancellationToken cancellationToken = default);

    Task<ServerResponseHelper<object>> UpdateUserAsync(
        string userId,
        UpdateTenantUserRequest request,
        CancellationToken cancellationToken = default);

    Task<ServerResponseHelper<object>> UpdateUserStatusAsync(
        string userId,
        UpdateTenantUserStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<ServerResponseHelper<object>> DeleteUserAsync(
        string userId,
        CancellationToken cancellationToken = default);
}