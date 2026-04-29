using Shala.Application.Common;
using Shala.Domain.Entities.Academics;

namespace Shala.Application.Repositories.Academics;

public interface ISectionRepository : IGenericRepository<Section>
{
    Task<List<Section>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<Section?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<List<Section>> GetByClassAsync(
        int tenantId,
        int branchId,
        int classId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        int tenantId,
        int branchId,
        int classId,
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default);
}