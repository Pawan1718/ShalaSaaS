using Shala.Application.Common;
using Shala.Domain.Entities.Fees;

namespace Shala.Application.Repositories.Fees;

public interface IStudentChargeRepository : IGenericRepository<StudentCharge>
{
    Task<List<StudentCharge>> GetByStudentIdAsync(int studentId, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<List<StudentCharge>> GetByAssignmentIdAsync(int studentFeeAssignmentId, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<StudentCharge?> GetByIdAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task AddRangeAsync(List<StudentCharge> entities, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<StudentCharge> charges, CancellationToken cancellationToken = default);
}