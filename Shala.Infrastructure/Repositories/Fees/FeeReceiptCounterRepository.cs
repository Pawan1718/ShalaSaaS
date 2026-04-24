using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Fees;

public sealed class FeeReceiptCounterRepository : IFeeReceiptCounterRepository
{
    private readonly AppDbContext _context;

    public FeeReceiptCounterRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<FeeReceiptCounter?> GetAsync(
        int tenantId,
        int branchId,
        int year,
        CancellationToken cancellationToken = default)
    {
        return _context.FeeReceiptCounters
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.Year == year,
                cancellationToken);
    }

    public async Task AddAsync(
        FeeReceiptCounter counter,
        CancellationToken cancellationToken = default)
    {
        await _context.FeeReceiptCounters.AddAsync(counter, cancellationToken);
    }

    public void Update(FeeReceiptCounter counter)
    {
        _context.FeeReceiptCounters.Update(counter);
    }
}