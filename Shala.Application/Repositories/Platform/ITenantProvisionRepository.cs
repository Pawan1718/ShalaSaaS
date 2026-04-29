using Shala.Domain.Entities.Identity;
using Shala.Domain.Entities.Organization;
using Shala.Domain.Entities.Platform;
using Shala.Shared.Common;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;

namespace Shala.Application.Repositories.Platform;

public interface ITenantProvisionRepository
{
    Task AddTenantAsync(SchoolTenant tenant, CancellationToken cancellationToken = default);
    Task AddBranchAsync(Branch branch, CancellationToken cancellationToken = default);
    Task AddUserBranchAccessAsync(UserBranchAccess access, CancellationToken cancellationToken = default);

    Task<bool> BranchCodeExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<List<TenantListItemResponse>> GetTenantsAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<TenantListItemResponse>> GetTenantsPagedAsync(TenantListRequest req, CancellationToken cancellationToken = default);

    Task<SchoolTenant?> GetTenantEntityByIdAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<TenantDetailResponse?> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default);

    Task<List<ApplicationUser>> GetTenantUsersAsync(int tenantId, CancellationToken cancellationToken = default);
}