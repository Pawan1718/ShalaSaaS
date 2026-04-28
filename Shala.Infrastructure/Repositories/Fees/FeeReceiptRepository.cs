using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Infrastructure.Data;
using Shala.Shared.Responses.Fees;

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



    public async Task<(IReadOnlyList<FeeReceiptResponse> Items, int TotalCount)> GetPagedByStudentIdAsync(
    int studentId,
    int tenantId,
    int branchId,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var query = _table
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                x.TenantId == tenantId &&
                x.BranchId == branchId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.ReceiptDate)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new FeeReceiptResponse
            {
                Id = x.Id,
                ReceiptNo = x.ReceiptNo,
                StudentId = x.StudentId,
                StudentAdmissionId = x.StudentAdmissionId ?? 0,
                ReceiptDate = x.ReceiptDate,
                PaymentMode = x.PaymentMode,
                TransactionReference = x.TransactionReference,
                Remarks = x.Remarks,
                TotalAmount = x.TotalAmount,
                IsCancelled = x.IsCancelled,
                CancelledOnUtc = x.CancelledOnUtc,
                CancelReason = x.CancelReason,

                Allocations = x.Allocations.Select(a => new FeeReceiptAllocationResponse
                {
                    StudentChargeId = a.StudentChargeId,
                    FeeHeadId = a.StudentCharge != null ? a.StudentCharge.FeeHeadId : 0,
                    ChargeLabel = a.StudentCharge != null ? a.StudentCharge.ChargeLabel : null,
                    FeeHeadName = a.StudentCharge != null && a.StudentCharge.FeeHead != null
                        ? a.StudentCharge.FeeHead.Name
                        : null,
                    PeriodLabel = a.StudentCharge != null ? a.StudentCharge.PeriodLabel : null,
                    DueDate = a.StudentCharge != null ? a.StudentCharge.DueDate : null,
                    AllocatedAmount = a.AllocatedAmount
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}