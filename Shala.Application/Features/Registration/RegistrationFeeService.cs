using Shala.Application.Repositories.Registration;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Application.Features.Registration
{
    public class RegistrationFeeService : IRegistrationFeeService
    {
        private readonly IRegistrationFeeRepository _repo;

        public RegistrationFeeService(IRegistrationFeeRepository repo)
        {
            _repo = repo;
        }

        public Task<RegistrationFeeResponse> CollectAsync(int tenantId, int branchId, int registrationId, CollectRegistrationFeeRequest request, CancellationToken ct)
        {
            return _repo.CollectAsync(tenantId, branchId, registrationId, request, ct);
        }

        public Task<RegistrationReceiptResponse> GetReceiptAsync(int tenantId, int branchId, int receiptId, CancellationToken ct)
        {
            return _repo.GetReceiptAsync(tenantId, branchId, receiptId, ct);
        }


        public Task CancelReceiptAsync(
    int tenantId,
    int branchId,
    int receiptId,
    string actor,
    CancelRegistrationReceiptRequest request,
    CancellationToken ct)
        {
            return _repo.CancelReceiptAsync(tenantId, branchId, receiptId, actor, request, ct);
        }

        public Task RefundReceiptAsync(
            int tenantId,
            int branchId,
            int receiptId,
            string actor,
            RefundRegistrationReceiptRequest request,
            CancellationToken ct)
        {
            return _repo.RefundReceiptAsync(tenantId, branchId, receiptId, actor, request, ct);
        }
    }
}