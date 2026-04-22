using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Registration;
using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.TenantConfig
{
    public class RegistrationProspectusConfigurationService : IRegistrationProspectusConfigurationService
    {
        private readonly IRegistrationProspectusConfigurationRepository _repo;

        public RegistrationProspectusConfigurationService(IRegistrationProspectusConfigurationRepository repo)
        {
            _repo = repo;
        }

        public async Task<RegistrationProspectusConfigurationResponse?> GetAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetActiveAsync(tenantId, branchId, cancellationToken);

            if (entity == null)
                return null;

            return Map(entity);
        }

        public async Task<RegistrationProspectusConfigurationResponse> SaveAsync(
            int tenantId,
            int branchId,
            SaveRegistrationProspectusConfigurationRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!request.IncludeProspectus && request.IsProspectusMandatory)
                throw new ArgumentException("Prospectus cannot be mandatory when include prospectus is disabled.");

            var entity = await _repo.GetByScopeAsync(tenantId, branchId, cancellationToken);

            if (entity == null)
            {
                entity = new RegistrationProspectusConfiguration
                {
                    TenantId = tenantId,
                    BranchId = branchId
                };

                await _repo.AddAsync(entity, cancellationToken);
            }

            entity.IncludeProspectus = request.IncludeProspectus;
            entity.ProspectusAmount = request.ProspectusAmount;
            entity.IsProspectusMandatory = request.IsProspectusMandatory;
            entity.ProspectusFeeHeadId = request.ProspectusFeeHeadId;
            entity.ProspectusDisplayName = string.IsNullOrWhiteSpace(request.ProspectusDisplayName)
                ? null
                : request.ProspectusDisplayName.Trim();
            entity.ShowProspectusInReceipt = request.ShowProspectusInReceipt;
            entity.IsActive = request.IsActive;

            await _repo.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        private static RegistrationProspectusConfigurationResponse Map(RegistrationProspectusConfiguration entity)
        {
            return new RegistrationProspectusConfigurationResponse
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                BranchId = entity.BranchId,
                IncludeProspectus = entity.IncludeProspectus,
                ProspectusAmount = entity.ProspectusAmount,
                IsProspectusMandatory = entity.IsProspectusMandatory,
                ProspectusFeeHeadId = entity.ProspectusFeeHeadId,
                ProspectusDisplayName = entity.ProspectusDisplayName,
                ShowProspectusInReceipt = entity.ShowProspectusInReceipt,
                IsActive = entity.IsActive
            };
        }
    }
}