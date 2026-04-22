using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Application.Repositories.Platform;
using Shala.Domain.Entities.Organization;
using Shala.Domain.Entities.Platform;
using Shala.Infrastructure.Data;
using Shala.Shared.Common;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;

namespace Shala.Infrastructure.Repositories.Platform;

public class TenantProvisionRepository : ITenantProvisionRepository
{
    private readonly AppDbContext _context;

    public TenantProvisionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddTenantAsync(SchoolTenant tenant, CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(tenant, cancellationToken);
    }

    public async Task AddBranchAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        await _context.Branches.AddAsync(branch, cancellationToken);
    }

    public async Task AddUserBranchAccessAsync(UserBranchAccess access, CancellationToken cancellationToken = default)
    {
        await _context.UserBranchAccesses.AddAsync(access, cancellationToken);
    }

    public async Task<bool> BranchCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Branches.AnyAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<List<TenantListItemResponse>> GetTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .OrderByDescending(x => x.Id)
            .Select(x => new TenantListItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                BusinessCategory = x.BusinessCategory,
                Email = x.Email,
                MobileNumber = x.MobileNumber,
                SubscriptionPlan = x.SubscriptionPlan,
                Subdomain = x.Subdomain,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                BranchCount = x.Branches.Count()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<TenantListItemResponse>> GetTenantsPagedAsync(TenantListRequest req, CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            var search = req.SearchText.Trim().ToLower();

            query = query.Where(x =>
                (x.Name ?? "").ToLower().Contains(search) ||
                (x.Email ?? "").ToLower().Contains(search) ||
                (x.BusinessCategory ?? "").ToLower().Contains(search) ||
                (x.SubscriptionPlan ?? "").ToLower().Contains(search));
        }

        if (req.IsActive.HasValue)
            query = query.Where(x => x.IsActive == req.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(req.SubscriptionPlan))
            query = query.Where(x => x.SubscriptionPlan == req.SubscriptionPlan);

        if (!string.IsNullOrWhiteSpace(req.BusinessCategory))
            query = query.Where(x => x.BusinessCategory == req.BusinessCategory);

        query = req.SortBy switch
        {
            "name" => req.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "email" => req.SortDescending ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
            "plan" => req.SortDescending ? query.OrderByDescending(x => x.SubscriptionPlan) : query.OrderBy(x => x.SubscriptionPlan),
            "category" => req.SortDescending ? query.OrderByDescending(x => x.BusinessCategory) : query.OrderBy(x => x.BusinessCategory),
            "status" => req.SortDescending ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
            "created" => req.SortDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };

        var mapped = query.Select(x => new TenantListItemResponse
        {
            Id = x.Id,
            Name = x.Name,
            BusinessCategory = x.BusinessCategory,
            Email = x.Email,
            SubscriptionPlan = x.SubscriptionPlan,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt,
            BranchCount = x.Branches.Count()
        });

        return await mapped.ToPagedResultAsync(req.PageNumber, req.PageSize);
    }

    public async Task<SchoolTenant?> GetTenantEntityByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);
    }

    public async Task<TenantDetailResponse?> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .Where(x => x.Id == tenantId)
            .Select(x => new TenantDetailResponse
            {
                Id = x.Id,
                SchoolName = x.Name,
                Email = x.Email,
                MobileNumber = x.MobileNumber,
                BusinessCategory = x.BusinessCategory,
                SubscriptionPlan = x.SubscriptionPlan,
                IsActive = x.IsActive,
                CreatedAtUtc = x.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}