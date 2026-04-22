using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Academics;
using Shala.Domain.Entities.Academics;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Academics;

public class SectionRepository : GenericRepository<Section>, ISectionRepository
{
    public SectionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Section>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.AcademicClass)
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Section?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.AcademicClass)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<List<Section>> GetByClassAsync(
        int tenantId,
        int branchId,
        int classId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicClassId == classId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        int tenantId,
        int branchId,
        int classId,
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _table.AnyAsync(x =>
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            x.AcademicClassId == classId &&
            x.Name == name &&
            (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}