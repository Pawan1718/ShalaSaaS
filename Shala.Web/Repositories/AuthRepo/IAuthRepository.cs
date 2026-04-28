using Shala.Shared.Requests.Identity;
using Shala.Shared.Responses.Identity;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.AuthRepo
{
    public interface IAuthRepository
    {
        Task<ServerResponseHelper<PlatformLoginResponse>> TenantLoginAsync(TenantLoginRequest request);
        Task<ServerResponseHelper<PlatformLoginResponse>> PlatformLoginAsync(PlatformLoginRequest request);
    }

}