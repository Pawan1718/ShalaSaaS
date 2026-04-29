using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;
using Shala.Web.Repositories.Base;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.StudentRepo
{
    public class StudentGuardianRepository : RepositoryBase, IStudentGuardianRepository
    {
        public StudentGuardianRepository(HttpClient httpClient, ApiSession session)
            : base(httpClient, session)
        {
        }

        public async Task<ApiResponse<GuardianResponse>?> AddAsync(int studentId, CreateGuardianRequest request)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.PostAsJsonAsync($"api/students/{studentId}/guardians", request);
            return await ReadApiResponse<ApiResponse<GuardianResponse>>(response, "Failed to add guardian.");
        }

        public async Task<ApiResponse<GuardianResponse>?> UpdateAsync(int studentId, int guardianId, CreateGuardianRequest request)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.PutAsJsonAsync($"api/students/{studentId}/guardians/{guardianId}", request);
            return await ReadApiResponse<ApiResponse<GuardianResponse>>(response, "Failed to update guardian.");
        }

        public async Task<ApiResponse<bool>?> RemoveAsync(int studentId, int guardianId, int tenantId)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.DeleteAsync($"api/students/{studentId}/guardians/{guardianId}?tenantId={tenantId}");
            return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to remove guardian.");
        }
    }
}
