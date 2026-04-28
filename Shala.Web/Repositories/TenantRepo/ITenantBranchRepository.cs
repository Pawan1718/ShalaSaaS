using Shala.Shared.Responses.Tenant;

namespace Shala.Web.Repositories.TenantRepo;

public interface ITenantBranchRepository
{
    Task<List<TenantBranchOptionDto>> GetMyBranchesAsync();
}