using Shala.Application.Common;
using Shala.Domain.Entities.Academics;

namespace Shala.Application.Repositories.Academics;

public interface IAcademicClassRepository : IGenericRepository<AcademicClass>
{
    Task<List<AcademicClass>> GetAllAsync(int tenantId, CancellationToken cancellationToken = default);

    Task<AcademicClass?> GetByIdAsync(
        int id,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        int tenantId,
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default);
}