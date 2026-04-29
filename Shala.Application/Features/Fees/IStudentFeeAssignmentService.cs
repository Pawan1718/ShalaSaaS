using Shala.Domain.Entities.Fees;

namespace Shala.Application.Features.Fees;

public interface IStudentFeeAssignmentService
{
    Task<List<StudentFeeAssignment>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<StudentFeeAssignment?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<List<StudentFeeAssignment>> GetByAdmissionIdAsync(
     int tenantId,
     int branchId,
     int studentAdmissionId,
     CancellationToken cancellationToken = default);

    Task<(bool Success, string Message, StudentFeeAssignment? Data)> AssignAsync(
        int tenantId,
        int branchId,
        StudentFeeAssignment entity,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> UpdateAsync(
        int tenantId,
        int branchId,
        StudentFeeAssignment entity,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<(bool CanModify, string Message)> CanModifyAssignmentAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);
}