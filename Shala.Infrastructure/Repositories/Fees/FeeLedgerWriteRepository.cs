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
            .Where(x => x.TenantId == tenantId
                        && x.BranchId == branchId
                        && x.StudentAdmissionId == studentAdmissionId)
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

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}