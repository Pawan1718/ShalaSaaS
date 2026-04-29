using Shala.Domain.Entities.Fees;
using Shala.Shared.Common;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public interface IFeeReceiptService
{
    Task<List<FeeReceipt>> GetByStudentIdAsync(
        int tenantId,
        int branchId,
        int studentId,
        CancellationToken cancellationToken = default);

    Task<FeeReceipt?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message, FeeReceipt? Data)> CreateAsync(
        int tenantId,
        int branchId,
        FeeReceipt entity,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> CancelReceiptAsync(
        int tenantId,
        int branchId,
        int id,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<FeeReceiptResponse>> GetPagedByStudentIdAsync(
        int tenantId,
        int branchId,
        int studentId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}