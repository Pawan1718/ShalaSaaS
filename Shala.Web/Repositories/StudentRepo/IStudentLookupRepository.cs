using Shala.Shared.Common;
using Shala.Shared.Responses.Students;

namespace Shala.Web.Repositories.Interfaces;

public interface IStudentLookupRepository
{
    Task<ApiResponse<List<LookupItemResponse>>?> GetAcademicYearsAsync(int tenantId);
    Task<ApiResponse<List<LookupItemResponse>>?> GetClassesAsync(int tenantId);
    Task<ApiResponse<List<LookupItemResponse>>?> GetSectionsByClassAsync(int tenantId, int branchId, int classId);
}