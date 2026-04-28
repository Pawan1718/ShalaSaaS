using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Fees;

public sealed class FeeLedgerWriteRepository : IFeeLedgerWriteRepository
{
    private readonly AppDbContext _db;

    public FeeLedgerWriteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<decimal> GetLatestRunningBalanceAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default)
    {
        return await _db.StudentFeeLedgers
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.StudentAdmissionId == studentAdmissionId)
            .OrderByDescending(x => x.EntryDate)
            .ThenByDescending(x => x.Id)
            .Select(x => (decimal?)x.RunningBalance)
            .FirstOrDefaultAsync(cancellationToken) ?? 0m;
    }

    public async Task AddRangeAsync(
        IEnumerable<StudentFeeLedger> entries,
        CancellationToken cancellationToken = default)
    {
        await _db.StudentFeeLedgers.AddRangeAsync(entries, cancellationToken);
    }

    public async Task<List<StudentFeeLedger>> GetByChargeIdsAsync(
        int tenantId,
        int branchId,
        IEnumerable<int> chargeIds,
        CancellationToken cancellationToken = default)
    {
        var ids = chargeIds
            .Where(x => x > 0)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return new List<StudentFeeLedger>();

        return await _db.StudentFeeLedgers
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.StudentChargeId.HasValue &&
                ids.Contains(x.StudentChargeId.Value))
            .OrderBy(x => x.EntryDate)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<StudentFeeLedger>> GetByReceiptIdAsync(
        int tenantId,
        int branchId,
        int receiptId,
        CancellationToken cancellationToken = default)
    {
        return await _db.StudentFeeLedgers
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.FeeReceiptId == receiptId)
            .OrderBy(x => x.EntryDate)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public void RemoveRange(IEnumerable<StudentFeeLedger> entries)
    {
        _db.StudentFeeLedgers.RemoveRange(entries);
    }

    public async Task DeleteByChargeIdsAsync(
        int tenantId,
        int branchId,
        IEnumerable<int> chargeIds,
        CancellationToken cancellationToken = default)
    {
        var rows = await GetByChargeIdsAsync(
            tenantId,
            branchId,
            chargeIds,
            cancellationToken);

        if (rows.Count == 0)
            return;

        _db.StudentFeeLedgers.RemoveRange(rows);
    }

    public async Task DeleteByAssignmentIdAsync(
        int tenantId,
        int branchId,
        int studentFeeAssignmentId,
        CancellationToken cancellationToken = default)
    {
        var chargeIds = await _db.StudentCharges
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.StudentFeeAssignmentId == studentFeeAssignmentId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (chargeIds.Count == 0)
            return;

        await DeleteByChargeIdsAsync(
            tenantId,
            branchId,
            chargeIds,
            cancellationToken);
    }

    public Task RebuildRunningBalanceAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default)
    {
        return RebuildRunningBalanceFromAsync(
            tenantId,
            branchId,
            studentAdmissionId,
            DateTime.MinValue,
            cancellationToken);
    }

    public async Task RebuildRunningBalanceFromAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        DateTime fromEntryDate,
        CancellationToken cancellationToken = default)
    {
        var previousBalance = await _db.StudentFeeLedgers
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.StudentAdmissionId == studentAdmissionId &&
                x.EntryDate < fromEntryDate)
            .OrderByDescending(x => x.EntryDate)
            .ThenByDescending(x => x.Id)
            .Select(x => (decimal?)x.RunningBalance)
            .FirstOrDefaultAsync(cancellationToken) ?? 0m;

        var rows = await _db.StudentFeeLedgers
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.StudentAdmissionId == studentAdmissionId &&
                x.EntryDate >= fromEntryDate)
            .OrderBy(x => x.EntryDate)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var runningBalance = previousBalance;

        foreach (var row in rows)
        {
            runningBalance += row.DebitAmount;
            runningBalance -= row.CreditAmount;
            row.RunningBalance = runningBalance;
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}