using Shala.Application.Repositories.Fees;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public sealed class FeeLedgerService : IFeeLedgerService
{
    private readonly IFeeLedgerReadRepository _readRepository;

    public FeeLedgerService(IFeeLedgerReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<FeeLedgerDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        FeeLedgerDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        request ??= new FeeLedgerDashboardRequest();

        if (request.PageNumber <= 0)
            request.PageNumber = 1;

        if (request.PageSize == 0)
            request.PageSize = 10;

        request.SortBy ??= "EntryDate";
        request.SortDescending = true;

        return await _readRepository.GetDashboardAsync(
            tenantId,
            branchId,
            request,
            cancellationToken);
    }
}