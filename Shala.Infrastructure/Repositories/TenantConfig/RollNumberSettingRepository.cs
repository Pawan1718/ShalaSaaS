using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Students;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.TenantConfig;

public class RollNumberSettingRepository : IRollNumberSettingRepository
{
    private readonly AppDbContext _context;

    public RollNumberSettingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RollNumberSetting?> GetByTenantIdAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RollNumberSettings
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }

    public async Task AddAsync(
        RollNumberSetting entity,
        CancellationToken cancellationToken = default)
    {
        await _context.RollNumberSettings.AddAsync(entity, cancellationToken);
    }

    public void Update(RollNumberSetting entity)
    {
        _context.RollNumberSettings.Update(entity);
    }
}