

using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Web.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<ApiResponse<PagedResult<StudentListItemResponse>>?> GetPagedAsync(StudentListRequest request);
    Task<ApiResponse<StudentDetailsResponse>?> GetByIdAsync(int id, int tenantId);
    Task<ApiResponse<StudentDetailsResponse>?> CreateAsync(CreateStudentRequest request);
    Task<ApiResponse<StudentDetailsResponse>?> UpdateAsync(int id, UpdateStudentRequest request);


    Task<ApiResponse<List<StudentAdmissionListItemResponse>>?> GetAdmissionsByAcademicYearAndClassAsync(
    int tenantId,
    int academicYearId,
    int classId);

}