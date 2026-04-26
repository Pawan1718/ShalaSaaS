using Shala.Application.Common;
using Shala.Domain.Entities.Academics;

namespace Shala.Application.Repositories.Academics;

public interface IAcademicYearRepository : IGenericRepository<AcademicYear>
{
    Task<List<AcademicYear>> GetAllAsync(int tenantId, CancellationToken cancellationToken = default);

    Task<AcademicYear?> GetByIdAsync(
        int id,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<AcademicYear?> GetCurrentAsync(int tenantId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        int tenantId,
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasDateOverlapAsync(
        int tenantId,
        DateTime startDate,
        DateTime endDate,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasAdmissionsAsync(
        int academicYearId,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task ResetCurrentAsync(
        int tenantId,
        int? excludeId = null,
        CancellationToken cancellationToken = default);
}