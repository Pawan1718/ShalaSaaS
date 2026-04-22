using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Registration;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.TenantConfig
{
    public class RegistrationReceiptConfigurationRepository : IRegistrationReceiptConfigurationRepository
    {
        private readonly AppDbContext _db;

        public RegistrationReceiptConfigurationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RegistrationReceiptConfiguration?> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _db.RegistrationReceiptConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.BranchId == branchId &&
                    x.IsActive,
                    cancellationToken);
        }

        public async Task<RegistrationReceiptConfiguration?> GetByScopeAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _db.RegistrationReceiptConfigurations
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.BranchId == branchId,
                    cancellationToken);
        }

        public async Task AddAsync(
            RegistrationReceiptConfiguration entity,
            CancellationToken cancellationToken = default)
        {
            await _db.RegistrationReceiptConfigurations.AddAsync(entity, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}