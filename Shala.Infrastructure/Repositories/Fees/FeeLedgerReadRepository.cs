using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Infrastructure.Data;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Infrastructure.Repositories.Fees;

public sealed class FeeLedgerReadRepository : IFeeLedgerReadRepository
{
    private readonly AppDbContext _db;

    public FeeLedgerReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<FeeLedgerDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        FeeLedgerDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var query =
            from ledger in _db.StudentFeeLedgers.AsNoTracking()
            join admission in _db.StudentAdmissions.AsNoTracking()
                on ledger.StudentAdmissionId equals admission.Id
            join student in _db.Students.AsNoTracking()
                on ledger.StudentId equals student.Id
            where ledger.TenantId == tenantId
                  && ledger.BranchId == branchId
            select new FeeLedgerRowResponse
            {
                Id = ledger.Id,
                StudentId = ledger.StudentId,
                StudentAdmissionId = ledger.StudentAdmissionId,
                StudentName =
                    ((student.FirstName ?? string.Empty) + " " +
                     (student.MiddleName ?? string.Empty) + " " +
                     (student.LastName ?? string.Empty)).Trim(),
                AdmissionNo = admission.AdmissionNo,
                EntryDate = ledger.EntryDate,
                EntryType = ledger.EntryType,
                DebitAmount = ledger.DebitAmount,
                CreditAmount = ledger.CreditAmount,
                RunningBalance = ledger.RunningBalance,
                ReferenceNo = ledger.ReferenceNo,
                Remarks = ledger.Remarks
            };

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim().ToLower();

            query = query.Where(x =>
                x.StudentName.ToLower().Contains(search) ||
                x.AdmissionNo.ToLower().Contains(search) ||
                (x.ReferenceNo != null && x.ReferenceNo.ToLower().Contains(search)) ||
                (x.Remarks != null && x.Remarks.ToLower().Contains(search)));
        }

        if (request.StudentId.HasValue)
            query = query.Where(x => x.StudentId == request.StudentId.Value);

        if (request.AdmissionId.HasValue)
            query = query.Where(x => x.StudentAdmissionId == request.AdmissionId.Value);

        if (!string.IsNullOrWhiteSpace(request.EntryType))
            query = query.Where(x => x.EntryType == request.EntryType);

        if (request.FromDate.HasValue)
        {
            var from = request.FromDate.Value.Date;
            query = query.Where(x => x.EntryDate >= from);
        }

        if (request.ToDate.HasValue)
        {
            var to = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.EntryDate <= to);
        }

        query = request.SortBy?.Trim().ToLowerInvariant() switch
        {
            "studentname" => request.SortDescending ? query.OrderByDescending(x => x.StudentName) : query.OrderBy(x => x.StudentName),
            "admissionno" => request.SortDescending ? query.OrderByDescending(x => x.AdmissionNo) : query.OrderBy(x => x.AdmissionNo),
            "entrytype" => request.SortDescending ? query.OrderByDescending(x => x.EntryType) : query.OrderBy(x => x.EntryType),
            "debitamount" => request.SortDescending ? query.OrderByDescending(x => x.DebitAmount) : query.OrderBy(x => x.DebitAmount),
            "creditamount" => request.SortDescending ? query.OrderByDescending(x => x.CreditAmount) : query.OrderBy(x => x.CreditAmount),
            "runningbalance" => request.SortDescending ? query.OrderByDescending(x => x.RunningBalance) : query.OrderBy(x => x.RunningBalance),
            _ => request.SortDescending ? query.OrderByDescending(x => x.EntryDate) : query.OrderBy(x => x.EntryDate)
        };

        var summary = new FeeLedgerDashboardSummaryResponse
        {
            TotalEntries = await query.CountAsync(cancellationToken),
            TotalDebit = await query.Select(x => x.DebitAmount).SumAsync(cancellationToken),
            TotalCredit = await query.Select(x => x.CreditAmount).SumAsync(cancellationToken),
            ClosingBalance = await query.Select(x => (decimal?)x.RunningBalance).FirstOrDefaultAsync(cancellationToken) ?? 0m
        };

        var paged = await query.ToPagedResultAsync(request.PageNumber, request.PageSize);

        return new FeeLedgerDashboardResponse
        {
            Summary = summary,
            Entries = paged
        };
    }
}