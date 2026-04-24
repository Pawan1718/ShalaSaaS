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

    Task DeleteByChargeIdsAsync(
        int tenantId,
        int branchId,
        IEnumerable<int> chargeIds,
        CancellationToken cancellationToken = default);

    Task DeleteByAssignmentIdAsync(
        int tenantId,
        int branchId,
        int studentFeeAssignmentId,
        CancellationToken cancellationToken = default);

    Task RebuildRunningBalanceAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default);

    Task RebuildRunningBalanceFromAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        DateTime fromEntryDate,
        CancellationToken cancellationToken = default);

    Task<List<StudentFeeLedger>> GetByChargeIdsAsync(
        int tenantId,
        int branchId,
        IEnumerable<int> chargeIds,
        CancellationToken cancellationToken = default);

    Task<List<StudentFeeLedger>> GetByReceiptIdAsync(
        int tenantId,
        int branchId,
        int receiptId,
        CancellationToken cancellationToken = default);

    void RemoveRange(IEnumerable<StudentFeeLedger> entries);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}