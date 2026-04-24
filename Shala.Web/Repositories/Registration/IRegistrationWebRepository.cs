using Shala.Shared.Common;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Web.Repositories.Registration
{
    public interface IRegistrationWebRepository
    {
        Task<int> CreateAsync(
            CreateRegistrationRequest request,
            CancellationToken cancellationToken = default);

        Task<PagedResult<RegistrationDto>> GetPagedAsync(
            PagedRequest request,
            CancellationToken cancellationToken = default);

        Task<List<RegistrationDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<RegistrationDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            int id,
            UpdateRegistrationRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);

      

        Task<RegistrationFeeResponse> SaveWithFeeAsync(
            SaveRegistrationWithFeeRequest request,
            CancellationToken cancellationToken = default);

        Task<RegistrationFeeResponse> CollectFeeAsync(
            int id,
            CollectRegistrationFeeRequest request,
            CancellationToken cancellationToken = default);

        Task<RegistrationReceiptResponse> GetReceiptAsync(
            int receiptId,
            CancellationToken cancellationToken = default);


        Task<ConvertRegistrationResponse> ConvertAsync(
    int id,
    ConvertRegistrationRequest request,
    CancellationToken cancellationToken = default);



        Task CancelReceiptAsync(
     int receiptId,
     CancelRegistrationReceiptRequest request,
     CancellationToken cancellationToken = default);

        Task RefundReceiptAsync(
            int receiptId,
            RefundRegistrationReceiptRequest request,
            CancellationToken cancellationToken = default);


    }
}