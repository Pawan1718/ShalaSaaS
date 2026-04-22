using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Registration;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.TenantConfig
{
    public class RegistrationFeeConfigurationRepository : IRegistrationFeeConfigurationRepository
    {
        private readonly AppDbContext _db;

        public RegistrationFeeConfigurationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RegistrationFeeConfiguration?> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _db.RegistrationFeeConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.BranchId == branchId &&
                    x.IsActive,
                    cancellationToken);
        }

        public async Task<RegistrationFeeConfiguration?> GetByScopeAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _db.RegistrationFeeConfigurations
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.BranchId == branchId,
                    cancellationToken);
        }

        public async Task AddAsync(
            RegistrationFeeConfiguration entity,
            CancellationToken cancellationToken = default)
        {
            await _db.RegistrationFeeConfigurations.AddAsync(entity, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}