using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Platform;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.TenantRepo;

public class TenantUserRepository : ITenantUserRepository
{
    private readonly IHttpService _http;

    public TenantUserRepository(IHttpService http)
    {
        _http = http;
    }

    public Task<ServerResponseHelper<List<UserListItemResponse>>> GetUsersAsync(
        CancellationToken cancellationToken = default)
    {
        return _http.GetAsync<List<UserListItemResponse>>(
            "api/tenant/users",
            cancellationToken);
    }

    public Task<ServerResponseHelper<object>> CreateUserAsync(
        CreateTenantUserRequest request,
        CancellationToken cancellationToken = default)
    {
        return _http.PostAsync<CreateTenantUserRequest, object>(
            "api/tenant/users",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<object>> UpdateUserAsync(
        string userId,
        UpdateTenantUserRequest request,
        CancellationToken cancellationToken = default)
    {
        return _http.PutAsync<UpdateTenantUserRequest, object>(
            $"api/tenant/users/{userId}",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<object>> UpdateUserStatusAsync(
        string userId,
        UpdateTenantUserStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        return _http.PutAsync<UpdateTenantUserStatusRequest, object>(
            $"api/tenant/users/{userId}/status",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<object>> DeleteUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync(
            $"api/tenant/users/{userId}",
            cancellationToken);
    }
}