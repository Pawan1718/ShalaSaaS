using Shala.Shared.Responses.Platform;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.PlatformRepo;

public interface IPlatformDashboardRepository
{
    Task<ServerResponseHelper<PlatformDashboardResponse>> GetSummaryAsync();
}