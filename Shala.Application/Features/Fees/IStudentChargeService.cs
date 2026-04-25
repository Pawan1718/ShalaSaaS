using Shala.Domain.Entities.Fees;
using Shala.Shared.Common;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public interface IStudentChargeService
{
    Task<List<StudentCharge>> GetByStudentIdAsync(
        int tenantId,
        int branchId,
        int studentId,
        CancellationToken cancellationToken = default);

    Task<List<StudentCharge>> GetByAssignmentIdAsync(
        int tenantId,
        int branchId,
        int studentFeeAssignmentId,
        CancellationToken cancellationToken = default);

    Task<StudentCharge?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> AddRangeAsync(
        List<StudentCharge> entities,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> MarkCancelledAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<StudentChargeResponse>> GetPagedByStudentIdAsync(
        int tenantId,
        int branchId,
        int studentId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}