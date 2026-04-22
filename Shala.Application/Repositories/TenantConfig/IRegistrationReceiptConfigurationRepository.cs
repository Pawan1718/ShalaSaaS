using Shala.Domain.Entities.Registration;

namespace Shala.Application.Repositories.TenantConfig
{
    public interface IRegistrationReceiptConfigurationRepository
    {
        Task<RegistrationReceiptConfiguration?> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<RegistrationReceiptConfiguration?> GetByScopeAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            RegistrationReceiptConfiguration entity,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}