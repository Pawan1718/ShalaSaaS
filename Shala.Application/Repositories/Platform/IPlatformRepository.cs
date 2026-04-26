using Shala.Domain.Entities.Platform;

namespace Shala.Application.Repositories.Platform;

public interface IPlatformRepository
{
    Task<List<SchoolTenant>> GetAllTenantsAsync(
        CancellationToken cancellationToken = default);
}