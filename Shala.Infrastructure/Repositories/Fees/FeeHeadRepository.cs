using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Infrastructure.Data;
using Shala.Shared.Common;

namespace Shala.Infrastructure.Repositories.Fees;

public class FeeHeadRepository : GenericRepository<FeeHead>, IFeeHeadRepository
{
    private readonly AppDbContext _db;

    public FeeHeadRepository(AppDbContext context) : base(context)
    {
        _db = context;
    }

    public async Task<List<FeeHead>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<FeeHead?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<FeeHead?> GetByCodeAsync(
        string code,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .FirstOrDefaultAsync(
                x => x.Code == code &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<bool> IsInUseAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        var isUsedInStructureItems = await _db.Set<FeeStructureItem>()
            .AnyAsync(
                x => x.FeeHeadId == id &&
                     x.FeeStructure.TenantId == tenantId &&
                     x.FeeStructure.BranchId == branchId,
                cancellationToken);

        if (isUsedInStructureItems)
            return true;

        var isUsedInStudentCharges = await _db.Set<StudentCharge>()
            .AnyAsync(
                x => x.FeeHeadId == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);

        return isUsedInStudentCharges;
    }

    public async Task<PagedResult<FeeHead>> GetPagedAsync(
        int tenantId,
        int branchId,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.GetPagedAsync(
            request,
            x => x.TenantId == tenantId && x.BranchId == branchId,
            new List<System.Linq.Expressions.Expression<Func<FeeHead, string>>>
            {
                x => x.Name,
                x => x.Code
            },
            q => q.OrderBy(x => x.Name),
            cancellationToken);
    }
}