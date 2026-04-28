using Shala.Application.Common;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Common;

namespace Shala.Application.Repositories.Fees;

public interface IFeeHeadRepository : IGenericRepository<FeeHead>
{
    Task<List<FeeHead>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<FeeHead?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<FeeHead?> GetByCodeAsync(
        string code,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<bool> IsInUseAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<FeeHead>> GetPagedAsync(
        int tenantId,
        int branchId,
        PagedRequest request,
        CancellationToken cancellationToken = default);
}