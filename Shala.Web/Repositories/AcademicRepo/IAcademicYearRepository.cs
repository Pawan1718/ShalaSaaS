using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;

namespace Shala.Web.Repositories.AcademicRepo;

public interface IAcademicYearRepository
{
    Task<ApiResponse<List<AcademicYearListItemResponse>>?> GetAllAsync(int tenantId);
    Task<ApiResponse<AcademicYearListItemResponse>?> GetByIdAsync(int id);
    Task<ApiResponse<int>?> CreateAsync(CreateAcademicYearRequest request);
    Task<ApiResponse<bool>?> UpdateAsync(UpdateAcademicYearRequest request);
    Task<ApiResponse<bool>?> DeleteAsync(int id);

    // add later when backend ready
    Task<ApiResponse<bool>?> SetCurrentAsync(int id, int tenantId);
}