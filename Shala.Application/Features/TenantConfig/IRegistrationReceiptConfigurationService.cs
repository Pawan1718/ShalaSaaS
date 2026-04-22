using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.TenantConfig
{
    public interface IRegistrationReceiptConfigurationService
    {
        Task<RegistrationReceiptConfigurationResponse?> GetAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<RegistrationReceiptConfigurationResponse> SaveAsync(
            int tenantId,
            int branchId,
            SaveRegistrationReceiptConfigurationRequest request,
            CancellationToken cancellationToken = default);
    }
}