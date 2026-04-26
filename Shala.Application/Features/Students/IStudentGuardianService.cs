using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Students;

public interface IStudentGuardianService
{
    Task<ApiResponse<GuardianResponse>> AddAsync(
        int tenantId,
        int branchId,
        string actor,
        int studentId,
        CreateGuardianRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<GuardianResponse>> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        int studentId,
        int guardianId,
        CreateGuardianRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> RemoveAsync(
        int tenantId,
        int branchId,
        int studentId,
        int guardianId,
        CancellationToken cancellationToken = default);
}