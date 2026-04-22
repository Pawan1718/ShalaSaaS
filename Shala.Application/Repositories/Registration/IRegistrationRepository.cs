using Shala.Domain.Entities.Registration;
using Shala.Shared.Common;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Application.Repositories.Registration
{
    public interface IRegistrationRepository
    {
        Task<int> CreateAsync(StudentRegistration entity, CancellationToken ct);

        Task UpdateAsync(StudentRegistration entity, CancellationToken ct);

        Task<StudentRegistration?> GetByIdAsync(int id, int tenantId, int branchId, CancellationToken ct);

        Task<RegistrationDto?> GetByIdDtoAsync(int tenantId, int branchId, int id, CancellationToken ct);

        Task<List<RegistrationDto>> GetAllAsync(int tenantId, int branchId, CancellationToken ct);

        Task<PagedResult<RegistrationDto>> GetPagedAsync(int tenantId, int branchId, PagedRequest request, CancellationToken ct);
        Task<ConvertRegistrationResponse> ConvertToStudentAsync(
                int tenantId,
                int branchId,
                int id,
                ConvertRegistrationRequest request,
                string actor,
                CancellationToken ct);
    }
}