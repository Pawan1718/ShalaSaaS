using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Application.Features.Registration
{
    public interface IRegistrationFeeService
    {
        Task<RegistrationFeeResponse> CollectAsync(
            int tenantId,
            int branchId,
            int registrationId,
            CollectRegistrationFeeRequest request,
            CancellationToken ct);

        Task<RegistrationReceiptResponse> GetReceiptAsync(
            int tenantId,
            int branchId,
            int receiptId,
            CancellationToken ct);
    }
}