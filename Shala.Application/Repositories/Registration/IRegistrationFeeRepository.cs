using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Application.Repositories.Registration
{
    public interface IRegistrationFeeRepository
    {
        Task<RegistrationFeeResponse> SaveWithFeeAsync(
            int tenantId,
            int branchId,
            SaveRegistrationWithFeeRequest request,
            CancellationToken ct);

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