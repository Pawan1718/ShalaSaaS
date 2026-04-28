using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public interface IFeeLedgerPostingService
{
    Task PostChargesAsync(
        int tenantId,
        int branchId,
        int studentId,
        int studentAdmissionId,
        IEnumerable<StudentChargeResponse> charges,
        CancellationToken cancellationToken = default);

    Task PostReceiptAsync(
        int tenantId,
        int branchId,
        FeeReceiptResponse receipt,
        IEnumerable<FeeReceiptAllocationResponse> allocations,
        CancellationToken cancellationToken = default);
}