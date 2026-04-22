using Shala.Shared.Common;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Application.Features.Registration
{
    public interface IRegistrationService
    {
        Task<int> CreateAsync(int tenantId, int branchId, CreateRegistrationRequest request, CancellationToken ct);
        Task<PagedResult<RegistrationDto>> GetPagedAsync(int tenantId, int branchId, PagedRequest request, CancellationToken ct);
        Task<List<RegistrationDto>> GetAllAsync(int tenantId, int branchId, CancellationToken ct);
        Task<RegistrationDto> GetByIdAsync(int tenantId, int branchId, int id, CancellationToken ct);
        Task UpdateAsync(int tenantId, int branchId, int id, UpdateRegistrationRequest request, CancellationToken ct);
        Task DeleteAsync(int tenantId, int branchId, int id, CancellationToken ct);
        Task<RegistrationFeeResponse> SaveWithFeeAsync(int tenantId, int branchId, SaveRegistrationWithFeeRequest request, CancellationToken ct);

        Task<ConvertRegistrationResponse> ConvertAsync(
            int tenantId,
            int branchId,
            int id,
            ConvertRegistrationRequest request,
            string actor,
            CancellationToken ct);

    }
}