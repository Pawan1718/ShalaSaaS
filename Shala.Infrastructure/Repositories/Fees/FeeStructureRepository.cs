using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Fees;

public class FeeStructureRepository : GenericRepository<FeeStructure>, IFeeStructureRepository
{
    public FeeStructureRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<FeeStructure>> GetAllAsync(int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.Items)
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<FeeStructure?> GetByIdAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return await _table
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<FeeStructure?> GetWithItemsAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.Items)
                .ThenInclude(x => x.FeeHead)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }
}