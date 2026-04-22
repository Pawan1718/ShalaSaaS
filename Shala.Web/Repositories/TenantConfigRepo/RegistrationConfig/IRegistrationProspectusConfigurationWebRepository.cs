using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Web.Repositories.TenantConfig
{
    public interface IRegistrationProspectusConfigurationWebRepository
    {
        Task<RegistrationProspectusConfigurationResponse?> GetAsync(
            CancellationToken cancellationToken = default);

        Task<RegistrationProspectusConfigurationResponse> SaveAsync(
            SaveRegistrationProspectusConfigurationRequest request,
            CancellationToken cancellationToken = default);
    }
}