using Shala.Shared.Responses.Platform;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.PlatformRepo;

public sealed class PlatformDashboardRepository : IPlatformDashboardRepository
{
    private readonly IHttpService _httpService;

    public PlatformDashboardRepository(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public Task<ServerResponseHelper<PlatformDashboardResponse>> GetSummaryAsync()
    {
        return _httpService.GetAsync<PlatformDashboardResponse>(
            "api/platform/dashboard/summary");
    }
}