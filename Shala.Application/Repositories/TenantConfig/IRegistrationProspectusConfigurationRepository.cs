using Shala.Domain.Entities.Registration;

namespace Shala.Application.Repositories.TenantConfig
{
    public interface IRegistrationProspectusConfigurationRepository
    {
        Task<RegistrationProspectusConfiguration?> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<RegistrationProspectusConfiguration?> GetByScopeAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            RegistrationProspectusConfiguration entity,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}