using Shala.Domain.Entities.Fees;

namespace Shala.Application.Repositories.Fees;

public interface IFeeReceiptCounterRepository
{
    Task<FeeReceiptCounter?> GetAsync(
        int tenantId,
        int branchId,
        int year,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        FeeReceiptCounter counter,
        CancellationToken cancellationToken = default);

    void Update(FeeReceiptCounter counter);
}