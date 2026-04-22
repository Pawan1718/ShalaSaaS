using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Registration;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.TenantConfig
{
    public class RegistrationProspectusConfigurationRepository : IRegistrationProspectusConfigurationRepository
    {
        private readonly AppDbContext _db;

        public RegistrationProspectusConfigurationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RegistrationProspectusConfiguration?> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _db.RegistrationProspectusConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.BranchId == branchId &&
                    x.IsActive,
                    cancellationToken);
        }

        public async Task<RegistrationProspectusConfiguration?> GetByScopeAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _db.RegistrationProspectusConfigurations
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.BranchId == branchId,
                    cancellationToken);
        }

        public async Task AddAsync(
            RegistrationProspectusConfiguration entity,
            CancellationToken cancellationToken = default)
        {
            await _db.RegistrationProspectusConfigurations.AddAsync(entity, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}