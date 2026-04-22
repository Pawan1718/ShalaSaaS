using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Registration;
using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.TenantConfig
{
    public class RegistrationFeeConfigurationService : IRegistrationFeeConfigurationService
    {
        private readonly IRegistrationFeeConfigurationRepository _repo;

        public RegistrationFeeConfigurationService(IRegistrationFeeConfigurationRepository repo)
        {
            _repo = repo;
        }

        public async Task<RegistrationFeeConfigurationResponse?> GetAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            // IMPORTANT:
            // Read scoped config even when inactive, otherwise inactive saved config is lost in UI.
            var entity = await _repo.GetByScopeAsync(tenantId, branchId, cancellationToken);

            if (entity == null)
                return null;

            return Map(entity);
        }

        public async Task<RegistrationFeeConfigurationResponse> SaveAsync(
            int tenantId,
            int branchId,
            SaveRegistrationFeeConfigurationRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetByScopeAsync(tenantId, branchId, cancellationToken);

            if (entity == null)
            {
                entity = new RegistrationFeeConfiguration
                {
                    TenantId = tenantId,
                    BranchId = branchId
                };

                await _repo.AddAsync(entity, cancellationToken);
            }

            entity.IsRegistrationModuleEnabled = request.IsRegistrationModuleEnabled;
            entity.RegistrationFeeAmount = request.RegistrationFeeAmount;
            entity.IsRegistrationFeeMandatory = request.IsRegistrationFeeMandatory;
            entity.RegistrationFeeHeadId = request.RegistrationFeeHeadId;
            entity.IsActive = request.IsActive;

            await _repo.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        private static RegistrationFeeConfigurationResponse Map(RegistrationFeeConfiguration entity)
        {
            return new RegistrationFeeConfigurationResponse
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                BranchId = entity.BranchId,
                IsRegistrationModuleEnabled = entity.IsRegistrationModuleEnabled,
                RegistrationFeeAmount = entity.RegistrationFeeAmount,
                IsRegistrationFeeMandatory = entity.IsRegistrationFeeMandatory,
                RegistrationFeeHeadId = entity.RegistrationFeeHeadId,
                IsActive = entity.IsActive
            };
        }
    }
}