using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Academics;

public interface IAcademicClassService
{
    Task<ApiResponse<List<AcademicClassListItemResponse>>> GetAllAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<AcademicClassListItemResponse>> GetByIdAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<int>> CreateAsync(
        int tenantId,
        CreateAcademicClassRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> UpdateAsync(
        int tenantId,
        string actor,
        UpdateAcademicClassRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(
        int tenantId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<List<LookupItemResponse>>> GetLookupAsync(
        int tenantId,
        CancellationToken cancellationToken = default);
}