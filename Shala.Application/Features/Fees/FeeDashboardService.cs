using Shala.Application.Repositories.Fees;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public sealed class FeeDashboardService : IFeeDashboardService
{
    private readonly IFeeDashboardReadRepository _readRepository;

    public FeeDashboardService(IFeeDashboardReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<FeeDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        FeeDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        request ??= new FeeDashboardRequest();

        if (request.PageNumber <= 0)
            request.PageNumber = 1;

        if (request.PageSize == 0)
            request.PageSize = 10;

        return await _readRepository.GetDashboardAsync(
            tenantId,
            branchId,
            request,
            cancellationToken);
    }
}