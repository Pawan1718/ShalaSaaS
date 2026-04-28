using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Platform;
using Shala.Domain.Entities.Platform;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Platform;

public sealed class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _context;

    public PlatformRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SchoolTenant>> GetAllTenantsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}