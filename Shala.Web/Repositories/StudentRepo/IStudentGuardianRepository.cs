using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;

namespace Shala.Web.Repositories.StudentRepo
{
    public interface IStudentGuardianRepository
    {
        Task<ApiResponse<GuardianResponse>?> AddAsync(int studentId, CreateGuardianRequest request);
        Task<ApiResponse<GuardianResponse>?> UpdateAsync(int studentId, int guardianId, CreateGuardianRequest request);
        Task<ApiResponse<bool>?> RemoveAsync(int studentId, int guardianId, int tenantId);
    }
}
