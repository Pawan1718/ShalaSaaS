using Shala.Domain.Entities.Students;

namespace Shala.Application.Repositories.TenantConfig;

public interface IRollNumberSettingRepository
{
    Task<RollNumberSetting?> GetByTenantIdAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        RollNumberSetting entity,
        CancellationToken cancellationToken = default);

    void Update(RollNumberSetting entity);
}