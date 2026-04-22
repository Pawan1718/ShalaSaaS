using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.TenantConfig
{
    public interface IRegistrationProspectusConfigurationService
    {
        Task<RegistrationProspectusConfigurationResponse?> GetAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<RegistrationProspectusConfigurationResponse> SaveAsync(
            int tenantId,
            int branchId,
            SaveRegistrationProspectusConfigurationRequest request,
            CancellationToken cancellationToken = default);
    }
}