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
        var baseQuery =
            from ledger in _db.StudentFeeLedgers.AsNoTracking()
            join admission in _db.StudentAdmissions.AsNoTracking()
                on ledger.StudentAdmissionId equals admission.Id
            join student in _db.Students.AsNoTracking()
                on ledger.StudentId equals student.Id
            where ledger.TenantId == tenantId && ledger.BranchId == branchId
            select new FeeLedgerRowResponse
            {
                Id = ledger.Id,
                StudentId = ledger.StudentId,
                StudentAdmissionId = ledger.StudentAdmissionId,
                StudentName = ((student.FirstName ?? string.Empty) + " " +
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

            baseQuery = baseQuery.Where(x =>
                x.StudentName.ToLower().Contains(search) ||
                x.AdmissionNo.ToLower().Contains(search) ||
                (x.ReferenceNo != null && x.ReferenceNo.ToLower().Contains(search)) ||
                (x.Remarks != null && x.Remarks.ToLower().Contains(search)));
        }

        if (request.StudentId.HasValue)
            baseQuery = baseQuery.Where(x => x.StudentId == request.StudentId.Value);

        if (request.AdmissionId.HasValue)
            baseQuery = baseQuery.Where(x => x.StudentAdmissionId == request.AdmissionId.Value);

        if (!string.IsNullOrWhiteSpace(request.EntryType))
            baseQuery = baseQuery.Where(x => x.EntryType == request.EntryType);

        if (request.FromDate.HasValue)
        {
            var from = request.FromDate.Value.Date;
            baseQuery = baseQuery.Where(x => x.EntryDate >= from);
        }

        if (request.ToDate.HasValue)
        {
            var to = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);
            baseQuery = baseQuery.Where(x => x.EntryDate <= to);
        }

        var totalEntries = await baseQuery.CountAsync(cancellationToken);
        var totalDebit = await baseQuery.SumAsync(x => (decimal?)x.DebitAmount, cancellationToken) ?? 0m;
        var totalCredit = await baseQuery.SumAsync(x => (decimal?)x.CreditAmount, cancellationToken) ?? 0m;

        var latestBalances = await baseQuery
            .GroupBy(x => x.StudentAdmissionId)
            .Select(g => g
                .OrderByDescending(x => x.EntryDate)
                .ThenByDescending(x => x.Id)
                .Select(x => (decimal?)x.RunningBalance)
                .FirstOrDefault())
            .ToListAsync(cancellationToken);

        var closingBalance = latestBalances.Sum(x => x ?? 0m);

        var sortedQuery = request.SortBy?.Trim().ToLowerInvariant() switch
        {
            "studentname" => request.SortDescending
                ? baseQuery.OrderByDescending(x => x.StudentName).ThenByDescending(x => x.EntryDate).ThenByDescending(x => x.Id)
                : baseQuery.OrderBy(x => x.StudentName).ThenBy(x => x.EntryDate).ThenBy(x => x.Id),

            "admissionno" => request.SortDescending
                ? baseQuery.OrderByDescending(x => x.AdmissionNo).ThenByDescending(x => x.EntryDate).ThenByDescending(x => x.Id)
                : baseQuery.OrderBy(x => x.AdmissionNo).ThenBy(x => x.EntryDate).ThenBy(x => x.Id),

            "entrytype" => request.SortDescending
                ? baseQuery.OrderByDescending(x => x.EntryType).ThenByDescending(x => x.EntryDate).ThenByDescending(x => x.Id)
                : baseQuery.OrderBy(x => x.EntryType).ThenBy(x => x.EntryDate).ThenBy(x => x.Id),

            "debitamount" => request.SortDescending
                ? baseQuery.OrderByDescending(x => x.DebitAmount).ThenByDescending(x => x.EntryDate).ThenByDescending(x => x.Id)
                : baseQuery.OrderBy(x => x.DebitAmount).ThenBy(x => x.EntryDate).ThenBy(x => x.Id),

            "creditamount" => request.SortDescending
                ? baseQuery.OrderByDescending(x => x.CreditAmount).ThenByDescending(x => x.EntryDate).ThenByDescending(x => x.Id)
                : baseQuery.OrderBy(x => x.CreditAmount).ThenBy(x => x.EntryDate).ThenBy(x => x.Id),

            "runningbalance" => request.SortDescending
                ? baseQuery.OrderByDescending(x => x.RunningBalance).ThenByDescending(x => x.EntryDate).ThenByDescending(x => x.Id)
                : baseQuery.OrderBy(x => x.RunningBalance).ThenBy(x => x.EntryDate).ThenBy(x => x.Id),

            _ => request.SortDescending
                ? baseQuery.OrderByDescending(x => x.EntryDate).ThenByDescending(x => x.Id)
                : baseQuery.OrderBy(x => x.EntryDate).ThenBy(x => x.Id)
        };

        var paged = await sortedQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);

        return new FeeLedgerDashboardResponse
        {
            Summary = new FeeLedgerDashboardSummaryResponse
            {
                TotalEntries = totalEntries,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                ClosingBalance = closingBalance
            },
            Entries = paged
        };
    }
}