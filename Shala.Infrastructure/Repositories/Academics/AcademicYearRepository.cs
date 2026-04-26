using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Academics;
using Shala.Domain.Entities.Academics;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Academics;

public class AcademicYearRepository : GenericRepository<AcademicYear>, IAcademicYearRepository
{
    public AcademicYearRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<AcademicYear>> GetAllAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<AcademicYear?> GetByIdAsync(
        int id,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _table.FirstOrDefaultAsync(
            x => x.Id == id && x.TenantId == tenantId,
            cancellationToken);
    }

    public async Task<AcademicYear?> GetCurrentAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Where(x => x.TenantId == tenantId && x.IsCurrent && x.IsActive)
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
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

    public async Task<bool> HasDateOverlapAsync(
        int tenantId,
        DateTime startDate,
        DateTime endDate,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _table.AnyAsync(x =>
            x.TenantId == tenantId &&
            (!excludeId.HasValue || x.Id != excludeId.Value) &&
            startDate <= x.EndDate &&
            endDate >= x.StartDate,
            cancellationToken);
    }

    public async Task<bool> HasAdmissionsAsync(
        int academicYearId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Where(x => x.Id == academicYearId && x.TenantId == tenantId)
            .AnyAsync(x => x.StudentAdmissions.Any(), cancellationToken);
    }

    public async Task ResetCurrentAsync(
        int tenantId,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var items = await _table
            .Where(x => x.TenantId == tenantId && (!excludeId.HasValue || x.Id != excludeId.Value))
            .ToListAsync(cancellationToken);

        foreach (var item in items)
            item.IsCurrent = false;
    }
}