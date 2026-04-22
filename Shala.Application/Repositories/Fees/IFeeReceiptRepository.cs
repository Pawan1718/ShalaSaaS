using Shala.Application.Common;
using Shala.Domain.Entities.Fees;

namespace Shala.Application.Repositories.Fees;

public interface IFeeReceiptRepository : IGenericRepository<FeeReceipt>
{
    Task<List<FeeReceipt>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<List<FeeReceipt>> GetByStudentIdAsync(
        int studentId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<FeeReceipt?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<FeeReceipt?> GetByReceiptNoAsync(
        string receiptNo,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);
}