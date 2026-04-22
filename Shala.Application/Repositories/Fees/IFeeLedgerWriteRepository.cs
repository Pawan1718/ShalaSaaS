using Shala.Domain.Entities.Fees;

namespace Shala.Application.Repositories.Fees;

public interface IFeeLedgerWriteRepository
{
    Task<decimal> GetLatestRunningBalanceAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<StudentFeeLedger> entries,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}