using Shala.Shared.Requests.Tenant;

namespace Shala.Application.Features.Identity;

public interface IUserService
{
    Task<(bool Success, object Data)> GetUsersAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, object Data)> CreateUserAsync(
        int tenantId,
        CreateTenantUserRequest request,
        CancellationToken cancellationToken = default);

    Task<(bool Success, object Data)> UpdateUserAsync(
        int tenantId,
        string userId,
        UpdateTenantUserRequest request,
        CancellationToken cancellationToken = default);

    Task<(bool Success, object Data)> UpdateUserStatusAsync(
        int tenantId,
        string userId,
        UpdateTenantUserStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<(bool Success, object Data)> DeleteUserAsync(
        int tenantId,
        string userId,
        CancellationToken cancellationToken = default);
}