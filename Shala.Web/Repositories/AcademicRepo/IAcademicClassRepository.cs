using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Responses.Academics;

namespace Shala.Web.Repositories.AcademicRepo;

public interface IAcademicClassRepository
{
    Task<ApiResponse<List<AcademicClassListItemResponse>>?> GetAllAsync(int tenantId);
    Task<ApiResponse<AcademicClassListItemResponse>?> GetByIdAsync(int id);
    Task<ApiResponse<int>?> CreateAsync(CreateAcademicClassRequest request);
    Task<ApiResponse<bool>?> UpdateAsync(UpdateAcademicClassRequest request);
    Task<ApiResponse<bool>?> DeleteAsync(int id);
}