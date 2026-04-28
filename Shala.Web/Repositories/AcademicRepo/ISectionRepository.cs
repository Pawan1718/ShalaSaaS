using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;

namespace Shala.Web.Repositories.AcademicRepo;

public interface ISectionRepository
{
    Task<ApiResponse<List<SectionListItemResponse>>?> GetAllAsync(int tenantId, int branchId);
    Task<ApiResponse<SectionListItemResponse>?> GetByIdAsync(int id);
    Task<ApiResponse<int>?> CreateAsync(CreateSectionRequest request);
    Task<ApiResponse<bool>?> UpdateAsync(UpdateSectionRequest request);
    Task<ApiResponse<bool>?> DeleteAsync(int id);
}