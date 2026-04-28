using Shala.Application.Common;
using Shala.Domain.Entities.Fees;

namespace Shala.Application.Repositories.Fees;

public interface IStudentFeeAssignmentRepository : IGenericRepository<StudentFeeAssignment>
{
    Task<List<StudentFeeAssignment>> GetAllAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<StudentFeeAssignment?> GetByIdAsync(int id, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<List<StudentFeeAssignment>> GetByAdmissionIdAsync(int studentAdmissionId, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<StudentFeeAssignment?> GetByAdmissionAndStructureIdAsync(
        int studentAdmissionId,
        int feeStructureId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<bool> IsFirstAdmissionForStudentAsync(
        int studentId,
        int studentAdmissionId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);
}