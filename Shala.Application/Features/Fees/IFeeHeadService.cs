using Shala.Domain.Entities.Fees;
using Shala.Shared.Common;

namespace Shala.Application.Features.Fees;

public interface IFeeHeadService
{
    Task<List<FeeHead>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<FeeHead?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message, FeeHead? Data)> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        FeeHead entity,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        FeeHead entity,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<FeeHead>> GetPagedAsync(
        int tenantId,
        int branchId,
        PagedRequest request,
        CancellationToken cancellationToken = default);
}