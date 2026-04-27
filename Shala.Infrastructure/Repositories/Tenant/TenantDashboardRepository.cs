using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Tenant;
using Shala.Domain.Enums;
using Shala.Infrastructure.Data;
using Shala.Shared.Responses.Tenant;

namespace Shala.Infrastructure.Repositories.Tenant;

public sealed class TenantDashboardRepository : ITenantDashboardRepository
{
    private readonly AppDbContext _db;

    public TenantDashboardRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TenantDashboardResponse> GetAsync(
        int tenantId,
        string userId,
        string role,
        int? requestedBranchId,
        bool isAllBranches,
        CancellationToken cancellationToken = default)
    {
        var allowedBranchIds = await GetAllowedBranchIdsAsync(
            tenantId,
            userId,
            role,
            cancellationToken);

        if (allowedBranchIds.Count == 0)
            throw new UnauthorizedAccessException("No branch access found.");

        List<int> scopeBranchIds;

        if (isAllBranches)
        {
            scopeBranchIds = allowedBranchIds;
        }
        else
        {
            var branchId = requestedBranchId ?? allowedBranchIds.First();

            if (!allowedBranchIds.Contains(branchId))
                throw new UnauthorizedAccessException("You are not allowed to access this branch.");

            scopeBranchIds = new List<int> { branchId };
        }

        var tenantName = await _db.Tenants
            .AsNoTracking()
            .Where(x => x.Id == tenantId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? "Shala CRM";

        var selectedBranch = await _db.Branches
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && scopeBranchIds.Contains(x.Id))
            .OrderByDescending(x => x.IsMainBranch)
            .ThenBy(x => x.Name)
            .Select(x => new
            {
                x.Name,
                x.Code
            })
            .FirstOrDefaultAsync(cancellationToken);

        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var trendStart = new DateTime(today.Year, today.Month, 1).AddMonths(-5);

        var totalStudents = await _db.Students
            .AsNoTracking()
            .CountAsync(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId),
                cancellationToken);

        var activeStudents = await _db.Students
            .AsNoTracking()
            .CountAsync(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                x.Status == StudentStatus.Active,
                cancellationToken);

        var currentAdmissions = await _db.StudentAdmissions
            .AsNoTracking()
            .CountAsync(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                x.IsCurrent,
                cancellationToken);

        var todayAdmissions = await _db.StudentAdmissions
            .AsNoTracking()
            .CountAsync(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                x.AdmissionDate >= today &&
                x.AdmissionDate < tomorrow,
                cancellationToken);

        var todayCollection = await _db.FeeReceipts
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                !x.IsCancelled &&
                x.ReceiptDate >= today &&
                x.ReceiptDate < tomorrow)
            .SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0m;

        var monthCollection = await _db.FeeReceipts
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                !x.IsCancelled &&
                x.ReceiptDate >= monthStart &&
                x.ReceiptDate < tomorrow)
            .SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0m;

        var totalOutstanding = await _db.StudentCharges
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                !x.IsCancelled)
            .SumAsync(x =>
                (decimal?)(x.Amount - x.DiscountAmount + x.FineAmount - x.PaidAmount),
                cancellationToken) ?? 0m;

        var defaulterCount = await _db.StudentCharges
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                !x.IsCancelled)
            .GroupBy(x => x.StudentAdmissionId)
            .Select(g => new
            {
                Balance = g.Sum(x => x.Amount - x.DiscountAmount + x.FineAmount - x.PaidAmount)
            })
            .CountAsync(x => x.Balance > 0, cancellationToken);

        var pendingDocuments = await _db.StudentDocumentChecklists
            .AsNoTracking()
            .CountAsync(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                x.IsActive &&
                !x.IsReceived &&
                x.DocumentModel.IsRequired,
                cancellationToken);

        var lowStockItems = await _db.SupplyItems
            .AsNoTracking()
            .CountAsync(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                x.IsActive &&
                x.CurrentStock <= x.MinimumStock,
                cancellationToken);

        var trendRaw = await _db.FeeReceipts
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                scopeBranchIds.Contains(x.BranchId) &&
                !x.IsCancelled &&
                x.ReceiptDate >= trendStart &&
                x.ReceiptDate < tomorrow)
            .GroupBy(x => new
            {
                x.ReceiptDate.Year,
                x.ReceiptDate.Month
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Amount = g.Sum(x => x.TotalAmount)
            })
            .ToListAsync(cancellationToken);

        var feeTrend = Enumerable.Range(0, 6)
            .Select(i => trendStart.AddMonths(i))
            .Select(month =>
            {
                var match = trendRaw.FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month);

                return new TenantDashboardTrendPointResponse
                {
                    Label = month.ToString("MMM"),
                    Amount = match?.Amount ?? 0m
                };
            })
            .ToList();

        var recentAdmissions = await (
            from admission in _db.StudentAdmissions.AsNoTracking()
            join branch in _db.Branches.AsNoTracking()
                on admission.BranchId equals branch.Id
            where admission.TenantId == tenantId &&
                  scopeBranchIds.Contains(admission.BranchId)
            orderby admission.AdmissionDate descending, admission.Id descending
            select new TenantDashboardRecentAdmissionResponse
            {
                AdmissionId = admission.Id,
                StudentName = (
                    admission.Student.FirstName + " " +
                    (admission.Student.MiddleName ?? string.Empty) + " " +
                    admission.Student.LastName).Trim(),
                Initials =
                    admission.Student.FirstName.Substring(0, 1) +
                    admission.Student.LastName.Substring(0, 1),
                ClassName = admission.AcademicClass.Name,
                SectionName = admission.Section != null ? admission.Section.Name : null,
                AdmissionNo = admission.AdmissionNo,
                BranchName = branch.Name,
                AdmissionDateText = admission.AdmissionDate.ToString("dd MMM yyyy")
            })
            .Take(8)
            .ToListAsync(cancellationToken);

        var response = new TenantDashboardResponse
        {
            TenantName = tenantName,
            SelectedBranchName = isAllBranches
                ? "All Branches"
                : selectedBranch?.Name ?? "No branch selected",
            SelectedBranchCode = isAllBranches ? null : selectedBranch?.Code,
            IsAllBranches = isAllBranches,

            TotalStudents = totalStudents,
            ActiveStudents = activeStudents,
            CurrentAdmissions = currentAdmissions,
            TodayAdmissions = todayAdmissions,

            TodayCollection = todayCollection,
            MonthCollection = monthCollection,
            TotalOutstanding = totalOutstanding,

            DefaulterCount = defaulterCount,
            PendingDocuments = pendingDocuments,
            LowStockItems = lowStockItems,

            FeeTrend = feeTrend,
            RecentAdmissions = recentAdmissions
        };

        response.Alerts = BuildAlerts(response);

        return response;
    }

    private async Task<List<int>> GetAllowedBranchIdsAsync(
        int tenantId,
        string userId,
        string role,
        CancellationToken cancellationToken)
    {
        if (IsTenantWideRole(role))
        {
            return await _db.Branches
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.IsActive)
                .OrderByDescending(x => x.IsMainBranch)
                .ThenBy(x => x.Name)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        return await _db.UserBranchAccesses
     .AsNoTracking()
     .Where(x =>
         x.TenantId == tenantId &&
         x.UserId == userId &&
         x.IsActive &&
         x.BranchId.HasValue)
     .Select(x => x.BranchId!.Value)
     .Distinct()
     .ToListAsync(cancellationToken);
    }

    private static bool IsTenantWideRole(string role)
    {
        return role.Equals("TenantAdmin", StringComparison.OrdinalIgnoreCase)
               || role.Equals("TenantOwner", StringComparison.OrdinalIgnoreCase)
               || role.Equals("Owner", StringComparison.OrdinalIgnoreCase)
               || role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }

    private static List<TenantDashboardAlertResponse> BuildAlerts(TenantDashboardResponse data)
    {
        var alerts = new List<TenantDashboardAlertResponse>();

        if (data.PendingDocuments > 0)
        {
            alerts.Add(new TenantDashboardAlertResponse
            {
                Title = "Pending Documents",
                Description = $"{data.PendingDocuments} documents need verification",
                Type = "documents"
            });
        }

        if (data.TotalOutstanding > 0)
        {
            alerts.Add(new TenantDashboardAlertResponse
            {
                Title = "Pending Fees",
                Description = $"₹ {data.TotalOutstanding:N0} outstanding",
                Type = "fees"
            });
        }

        if (data.DefaulterCount > 0)
        {
            alerts.Add(new TenantDashboardAlertResponse
            {
                Title = "Fee Defaulters",
                Description = $"{data.DefaulterCount} students have pending balance",
                Type = "fees"
            });
        }

        if (data.LowStockItems > 0)
        {
            alerts.Add(new TenantDashboardAlertResponse
            {
                Title = "Low Stock",
                Description = $"{data.LowStockItems} supply items need refill",
                Type = "supplies"
            });
        }

        return alerts.Take(4).ToList();
    }
}