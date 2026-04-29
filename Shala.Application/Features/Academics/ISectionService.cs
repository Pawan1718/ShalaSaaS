using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Academics;

public interface ISectionService
{
    Task<ApiResponse<List<SectionListItemResponse>>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SectionListItemResponse>> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<int>> CreateAsync(
        int tenantId,
        int branchId,
        CreateSectionRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        UpdateSectionRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<List<LookupItemResponse>>> GetLookupByClassAsync(
        int tenantId,
        int branchId,
        int classId,
        CancellationToken cancellationToken = default);
}