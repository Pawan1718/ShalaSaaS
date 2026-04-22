using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.TenantConfig
{
    public interface IRegistrationFeeConfigurationService
    {
        Task<RegistrationFeeConfigurationResponse?> GetAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<RegistrationFeeConfigurationResponse> SaveAsync(
            int tenantId,
            int branchId,
            SaveRegistrationFeeConfigurationRequest request,
            CancellationToken cancellationToken = default);
    }
}