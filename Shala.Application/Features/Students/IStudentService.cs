using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Students;

public interface IStudentService
{
    Task<ApiResponse<StudentDetailsResponse>> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        CreateStudentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<StudentDetailsResponse>> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        int id,
        UpdateStudentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<StudentDetailsResponse>> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResult<StudentListItemResponse>>> GetPagedAsync(
        int tenantId,
        int branchId,
        StudentListRequest request,
        CancellationToken cancellationToken = default);
}