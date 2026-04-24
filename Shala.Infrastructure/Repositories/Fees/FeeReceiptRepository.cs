using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Fees;

public class FeeReceiptRepository : GenericRepository<FeeReceipt>, IFeeReceiptRepository
{
    public FeeReceiptRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<FeeReceipt>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.Allocations)
                .ThenInclude(x => x.StudentCharge)
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId)
            .OrderByDescending(x => x.ReceiptDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FeeReceipt>> GetByStudentIdAsync(
        int studentId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.Allocations)
                .ThenInclude(x => x.StudentCharge)
            .Where(x =>
                x.StudentId == studentId &&
                x.TenantId == tenantId &&
                x.BranchId == branchId)
            .OrderByDescending(x => x.ReceiptDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<FeeReceipt?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.Allocations)
                .ThenInclude(x => x.StudentCharge)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<FeeReceipt?> GetByReceiptNoAsync(
        string receiptNo,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.Allocations)
                .ThenInclude(x => x.StudentCharge)
            .FirstOrDefaultAsync(
                x => x.ReceiptNo == receiptNo &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }
}