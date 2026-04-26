using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Web.Repositories.StudentRepo
{
    public interface IStudentAdmissionRepository
    {
        Task<ApiResponse<StudentAdmissionResponse>?> CreateAsync(CreateStudentAdmissionRequest request);
        Task<ApiResponse<StudentAdmissionResponse>?> UpdateAsync(int id, UpdateStudentAdmissionRequest request);
        Task<ApiResponse<bool>?> DeleteAsync(int id, int tenantId, int branchId);


        Task<ApiResponse<SectionRollAssignmentDetailResponse>?> GetAssignmentDetailAsync(int admissionId);
        Task<ApiResponse<SectionRollAssignmentPreviewResponse>?> GetAssignmentPreviewAsync(int admissionId, int? sectionId, bool autoGenerateRollNo, string? rollNo);
        Task<ApiResponse<StudentAdmissionResponse>?> AssignSectionRollAsync(AssignSectionRollRequest request);

        Task<ApiResponse<BulkSectionRollAssignmentPreviewResponse>?> GetBulkAssignmentPreviewAsync(BulkSectionRollAssignmentRequest request);
        Task<ApiResponse<List<StudentAdmissionResponse>>?> ApplyBulkAssignmentAsync(BulkSectionRollAssignmentRequest request);

        Task<ApiResponse<List<StudentAdmissionListItemResponse>>?> GetAdmissionsByAcademicYearAndClassAsync(
    int academicYearId,
    int classId);
    }
}
