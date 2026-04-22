using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Web.Repositories.TenantConfig
{
    public interface IRegistrationFeeConfigurationWebRepository
    {
        Task<RegistrationFeeConfigurationResponse?> GetAsync(
            CancellationToken cancellationToken = default);

        Task<RegistrationFeeConfigurationResponse> SaveAsync(
            SaveRegistrationFeeConfigurationRequest request,
            CancellationToken cancellationToken = default);
    }
}