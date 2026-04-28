using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Academics;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.TenantConfig;

public class AcademicYearSettingRepository : IAcademicYearSettingRepository
{
    private readonly AppDbContext _context;

    public AcademicYearSettingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AcademicYearSetting?> GetByTenantIdAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AcademicYearSettings
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }

    public async Task AddAsync(
        AcademicYearSetting entity,
        CancellationToken cancellationToken = default)
    {
        await _context.AcademicYearSettings.AddAsync(entity, cancellationToken);
    }

    public void Update(AcademicYearSetting entity)
    {
        _context.AcademicYearSettings.Update(entity);
    }
}