using Shala.Shared.Common;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Platform;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.TenantRepo
{
    public class TenantUserRepository
    {
        private readonly IHttpService _http;

        public TenantUserRepository(IHttpService http)
        {
            _http = http;
        }

        public async Task<ServerResponseHelper<List<UserListItemResponse>>> GetUsersAsync()
        {
            return await _http.GetAsync<List<UserListItemResponse>>("api/tenant/users");
        }

        public async Task<ServerResponseHelper<object>> CreateUserAsync(CreateTenantUserRequest req)
        {
            return await _http.PostAsync<CreateTenantUserRequest, object>("api/tenant/users", req);
        }
    }
}