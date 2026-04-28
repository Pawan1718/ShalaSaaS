using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Students;

public interface IStudentAdmissionService
{
    Task<ApiResponse<StudentAdmissionResponse>> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        CreateStudentAdmissionRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<StudentAdmissionResponse>> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        int id,
        UpdateStudentAdmissionRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SectionRollAssignmentDetailResponse>> GetAssignmentDetailAsync(
    int tenantId,
    int branchId,
    int admissionId,
    CancellationToken cancellationToken = default);

    Task<ApiResponse<SectionRollAssignmentPreviewResponse>> GetAssignmentPreviewAsync(
        int tenantId,
        int branchId,
        SectionRollAssignmentPreviewRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<StudentAdmissionResponse>> AssignSectionAndRollNoAsync(
        int tenantId,
        int branchId,
        string actor,
        AssignSectionRollRequest request,
        CancellationToken cancellationToken = default);


    Task<ApiResponse<BulkSectionRollAssignmentPreviewResponse>> GetBulkAssignmentPreviewAsync(
    int tenantId,
    int branchId,
    BulkSectionRollAssignmentRequest request,
    CancellationToken cancellationToken = default);

    Task<ApiResponse<List<StudentAdmissionResponse>>> ApplyBulkAssignmentAsync(
        int tenantId,
        int branchId,
        string actor,
        BulkSectionRollAssignmentRequest request,
        CancellationToken cancellationToken = default);




    Task<ApiResponse<List<StudentAdmissionListItemResponse>>> GetAdmissionsByAcademicYearAndClassAsync(
    int tenantId,
    int branchId,
    int academicYearId,
    int classId,
    CancellationToken cancellationToken = default);
}