using Shala.Shared.Requests.Identity;
using Shala.Shared.Responses.Identity;

namespace Shala.Application.Features.Platform;

public interface IPlatformAuthService
{
    Task<(bool Success, PlatformLoginResponse? Data, string Message)> LoginAsync(
        PlatformLoginRequest request,
        CancellationToken cancellationToken = default);
}