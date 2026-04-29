using Shala.Domain.Entities.Academics;

namespace Shala.Application.Repositories.TenantConfig;

public interface IAcademicYearSettingRepository
{
    Task<AcademicYearSetting?> GetByTenantIdAsync(int tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(AcademicYearSetting entity, CancellationToken cancellationToken = default);
    void Update(AcademicYearSetting entity);
}