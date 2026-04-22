using Shala.Application.Common;
using Shala.Domain.Entities.Students;

namespace Shala.Application.Repositories.Students;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<Student?> GetDetailsAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(
        int tenantId,
        int branchId,
        string? search,
        int? academicYearId,
        int? classId,
        int? sectionId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);




}