using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Platform;
using Shala.Domain.Entities.Platform;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Platform;

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    public BranchRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Branch?> GetByIdAsync(int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        return await _table
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == branchId,
                cancellationToken);
    }

    public async Task<List<Branch>> GetAllAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _table
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.IsMainBranch)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(int tenantId, string code, int? excludeBranchId = null, CancellationToken cancellationToken = default)
    {
        return await _table.AnyAsync(x =>
            x.TenantId == tenantId &&
            x.Code == code &&
            (!excludeBranchId.HasValue || x.Id != excludeBranchId.Value),
            cancellationToken);
    }
}