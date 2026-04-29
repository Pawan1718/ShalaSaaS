using Shala.Shared.Requests.Identity;
using Shala.Shared.Responses.Identity;

namespace Shala.Application.Features.Identity;

public interface ITenantAuthService
{
    Task<(bool Success, PlatformLoginResponse? Data, string Message)> LoginAsync(
        TenantLoginRequest request,
        CancellationToken cancellationToken = default);
}