using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Academics;
using Shala.Domain.Entities.Academics;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Academics;

public class AcademicClassRepository : GenericRepository<AcademicClass>, IAcademicClassRepository
{
    public AcademicClassRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<AcademicClass>> GetAllAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Sequence)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<AcademicClass?> GetByIdAsync(
        int id,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .FirstOrDefaultAsync(
                x => x.Id == id && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        int tenantId,
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _table.AnyAsync(x =>
            x.TenantId == tenantId &&
            x.Name == name &&
            (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}