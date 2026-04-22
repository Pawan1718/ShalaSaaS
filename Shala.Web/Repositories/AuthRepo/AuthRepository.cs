using Shala.Shared.Requests.Identity;
using Shala.Shared.Responses.Identity;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.AuthRepo;

public class AuthRepository : IAuthRepository
{
    private readonly IHttpService _httpService;

    public AuthRepository(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public Task<ServerResponseHelper<PlatformLoginResponse>> TenantLoginAsync(TenantLoginRequest request)
    {
        return _httpService.PostAsync<TenantLoginRequest, PlatformLoginResponse>(
            "api/tenant/auth/login",
            request);
    }

    public Task<ServerResponseHelper<PlatformLoginResponse>> PlatformLoginAsync(PlatformLoginRequest request)
    {
        return _httpService.PostAsync<PlatformLoginRequest, PlatformLoginResponse>(
            "api/platform/auth/login",
            request);
    }
}