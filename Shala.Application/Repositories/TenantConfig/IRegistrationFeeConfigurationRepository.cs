using Shala.Domain.Entities.Registration;

namespace Shala.Application.Repositories.TenantConfig
{
    public interface IRegistrationFeeConfigurationRepository
    {
        Task<RegistrationFeeConfiguration?> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<RegistrationFeeConfiguration?> GetByScopeAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            RegistrationFeeConfiguration entity,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}