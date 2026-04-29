using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Settings;
using Shala.Domain.Entities.Settings;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Settings
{
    public class BranchDocumentProfileRepository : IBranchDocumentProfileRepository
    {
        private readonly AppDbContext _db;

        public BranchDocumentProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<BranchDocumentProfile?> GetByScopeAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _db.Set<BranchDocumentProfile>()
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId && x.BranchId == branchId,
                    cancellationToken);
        }

        public async Task<BranchDocumentProfile?> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _db.Set<BranchDocumentProfile>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId &&
                         x.BranchId == branchId &&
                         x.IsActive,
                    cancellationToken);
        }

        public async Task AddAsync(
            BranchDocumentProfile entity,
            CancellationToken cancellationToken = default)
        {
            await _db.Set<BranchDocumentProfile>().AddAsync(entity, cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}