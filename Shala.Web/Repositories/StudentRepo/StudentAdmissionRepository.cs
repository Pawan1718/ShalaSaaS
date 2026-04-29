using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;
using Shala.Web.Repositories.Base;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.StudentRepo
{
    public class StudentAdmissionRepository : RepositoryBase, IStudentAdmissionRepository
    {
        public StudentAdmissionRepository(HttpClient httpClient, ApiSession session)
            : base(httpClient, session)
        {
        }

        public async Task<ApiResponse<StudentAdmissionResponse>?> CreateAsync(CreateStudentAdmissionRequest request)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.PostAsJsonAsync("api/student-admissions", request);
            return await ReadApiResponse<ApiResponse<StudentAdmissionResponse>>(response, "Failed to create admission.");
        }

        public async Task<ApiResponse<StudentAdmissionResponse>?> UpdateAsync(int id, UpdateStudentAdmissionRequest request)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.PutAsJsonAsync($"api/student-admissions/{id}", request);
            return await ReadApiResponse<ApiResponse<StudentAdmissionResponse>>(response, "Failed to update admission.");
        }

        public async Task<ApiResponse<bool>?> DeleteAsync(int id, int tenantId, int branchId)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.DeleteAsync($"api/student-admissions/{id}?tenantId={tenantId}&branchId={branchId}");
            return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to delete admission.");
        }




        public async Task<ApiResponse<SectionRollAssignmentDetailResponse>?> GetAssignmentDetailAsync(int admissionId)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.GetAsync($"api/student-admissions/{admissionId}/assignment-detail");
            return await ReadApiResponse<ApiResponse<SectionRollAssignmentDetailResponse>>(response, "Failed to load assignment detail.");
        }

        public async Task<ApiResponse<SectionRollAssignmentPreviewResponse>?> GetAssignmentPreviewAsync(
            int admissionId,
            int? sectionId,
            bool autoGenerateRollNo,
            string? rollNo)
        {
            await EnsureAuthAsync();

            var url = $"api/student-admissions/{admissionId}/assignment-preview?autoGenerateRollNo={autoGenerateRollNo.ToString().ToLowerInvariant()}";

            if (sectionId.HasValue)
                url += $"&sectionId={sectionId.Value}";

            if (!string.IsNullOrWhiteSpace(rollNo))
                url += $"&rollNo={Uri.EscapeDataString(rollNo)}";

            var response = await HttpClient.GetAsync(url);
            return await ReadApiResponse<ApiResponse<SectionRollAssignmentPreviewResponse>>(response, "Failed to load assignment preview.");
        }

        public async Task<ApiResponse<StudentAdmissionResponse>?> AssignSectionRollAsync(AssignSectionRollRequest request)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.PutAsJsonAsync("api/student-admissions/assign-section-roll", request);
            return await ReadApiResponse<ApiResponse<StudentAdmissionResponse>>(response, "Failed to assign section and roll number.");
        }

        public async Task<ApiResponse<BulkSectionRollAssignmentPreviewResponse>?> GetBulkAssignmentPreviewAsync(
            BulkSectionRollAssignmentRequest request)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.PostAsJsonAsync("api/student-admissions/bulk-assignment-preview", request);
            return await ReadApiResponse<ApiResponse<BulkSectionRollAssignmentPreviewResponse>>(response, "Failed to load bulk assignment preview.");
        }

        public async Task<ApiResponse<List<StudentAdmissionResponse>>?> ApplyBulkAssignmentAsync(
            BulkSectionRollAssignmentRequest request)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.PutAsJsonAsync("api/student-admissions/bulk-assign-section-roll", request);
            return await ReadApiResponse<ApiResponse<List<StudentAdmissionResponse>>>(response, "Failed to apply bulk assignment.");
        }

        public async Task<ApiResponse<List<StudentAdmissionListItemResponse>>?> GetAdmissionsByAcademicYearAndClassAsync(
            int academicYearId,
            int classId)
        {
            await EnsureAuthAsync();

            var response = await HttpClient.GetAsync(
                $"api/student-admissions/by-academic-year-class?academicYearId={academicYearId}&classId={classId}");

            return await ReadApiResponse<ApiResponse<List<StudentAdmissionListItemResponse>>>(
                response,
                "Failed to load admissions by academic year and class.");
        }
    }
}
