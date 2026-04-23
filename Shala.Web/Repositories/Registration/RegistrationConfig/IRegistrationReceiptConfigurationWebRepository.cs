using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Web.Repositories.Registration.RegistrationConfig
{
    public interface IRegistrationReceiptConfigurationWebRepository
    {
        Task<RegistrationReceiptConfigurationResponse?> GetAsync(
            CancellationToken cancellationToken = default);

        Task<RegistrationReceiptConfigurationResponse> SaveAsync(
            SaveRegistrationReceiptConfigurationRequest request,
            CancellationToken cancellationToken = default);
    }
}