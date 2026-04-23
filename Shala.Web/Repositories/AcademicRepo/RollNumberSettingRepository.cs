using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;
using Shala.Shared.Responses.TenantConfigSetting;
using Shala.Web.Repositories.Base;
using Shala.Web.Services.State;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Shala.Web.Repositories.AcademicRepo
{
    public class RollNumberSettingRepository : RepositoryBase, IRollNumberSettingRepository
    {
        private const string BaseRoute = "api/students/roll-number-settings";

        public RollNumberSettingRepository(HttpClient httpClient, ApiSession session)
            : base(httpClient, session)
        {
        }

        public async Task<ApiResponse<RollNumberSettingResponse>?> GetAsync(int tenantId)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.GetAsync($"{BaseRoute}?tenantId={tenantId}");
            return await ReadApiResponse<ApiResponse<RollNumberSettingResponse>>(response, "Failed to load roll number settings.");
        }

        public async Task<ApiResponse<bool>?> SaveAsync(SaveRollNumberSettingRequest request)
        {
            await EnsureAuthAsync();
            var response = await HttpClient.PostAsJsonAsync(BaseRoute, request);
            return await ReadApiResponse<ApiResponse<bool>>(response, "Failed to save roll number settings.");
        }
    }
}
