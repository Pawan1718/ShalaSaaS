using Shala.Application.Common;
using Shala.Domain.Entities.Supplies;

namespace Shala.Application.Repositories.Supplies;

public interface IStudentSupplyIssueRepository : IGenericRepository<StudentSupplyIssue>
{
    Task<StudentSupplyIssue?> GetDetailAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<List<StudentSupplyIssue>> GetRecentAsync(int tenantId, int branchId, int? academicYearId = null, CancellationToken cancellationToken = default);

    Task<List<StudentSupplyIssue>> GetStudentHistoryAsync(int tenantId, int branchId, int studentId, int? academicYearId = null, CancellationToken cancellationToken = default);

    Task<List<StudentSupplyIssue>> GetPendingDuesAsync(int tenantId, int branchId, int? academicYearId = null, int? studentId = null, CancellationToken cancellationToken = default);

    Task<int> GetTodayIssueCountAsync(int tenantId, int branchId, DateTime date, CancellationToken cancellationToken = default);

    Task<decimal> GetPendingDueTotalAsync(int tenantId, int branchId, int? academicYearId = null, CancellationToken cancellationToken = default);

    Task<string> GenerateNextIssueNoAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);
}