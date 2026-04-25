using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Application.Repositories.Reports;
using Shala.Domain.Enums;
using Shala.Infrastructure.Data;
using Shala.Shared.Common;
using Shala.Shared.Requests.Reports;
using Shala.Shared.Responses.Reports;

namespace Shala.Infrastructure.Repositories.Reports;

public sealed class ReportsRepository : IReportsRepository
{
    private readonly AppDbContext _db;

    public ReportsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ReportsDashboardResponse> GetDashboardAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var totalStudents = await _db.Students
            .AsNoTracking()
            .CountAsync(x => x.TenantId == tenantId && x.BranchId == branchId, cancellationToken);

        var activeStudents = await _db.Students
            .AsNoTracking()
            .CountAsync(x => x.TenantId == tenantId && x.BranchId == branchId && x.Status == StudentStatus.Active, cancellationToken);

        var currentAdmissions = await _db.StudentAdmissions
            .AsNoTracking()
            .CountAsync(x => x.TenantId == tenantId && x.BranchId == branchId && x.IsCurrent, cancellationToken);

        var todayAdmissions = await _db.StudentAdmissions
            .AsNoTracking()
            .CountAsync(x => x.TenantId == tenantId && x.BranchId == branchId && x.AdmissionDate >= today && x.AdmissionDate < tomorrow, cancellationToken);

        var todayCollection = await _db.FeeReceipts
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId && !x.IsCancelled && x.ReceiptDate >= today && x.ReceiptDate < tomorrow)
            .SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0m;

        var monthCollection = await _db.FeeReceipts
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId && !x.IsCancelled && x.ReceiptDate >= monthStart && x.ReceiptDate < tomorrow)
            .SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0m;

        var chargeSummary = await _db.StudentCharges
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId && !x.IsCancelled)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Balance = g.Sum(x => x.Amount - x.DiscountAmount + x.FineAmount - x.PaidAmount)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var defaulterCount = await _db.StudentCharges
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId && !x.IsCancelled)
            .GroupBy(x => x.StudentAdmissionId)
            .Select(g => new
            {
                Balance = g.Sum(x => x.Amount - x.DiscountAmount + x.FineAmount - x.PaidAmount)
            })
            .CountAsync(x => x.Balance > 0, cancellationToken);

        var pendingDocuments = await _db.StudentDocumentChecklists
            .AsNoTracking()
            .CountAsync(x => x.TenantId == tenantId && x.BranchId == branchId && x.IsActive && !x.IsReceived && x.DocumentModel.IsRequired, cancellationToken);

        return new ReportsDashboardResponse
        {
            TotalStudents = totalStudents,
            ActiveStudents = activeStudents,
            CurrentAdmissions = currentAdmissions,
            TodayAdmissions = todayAdmissions,
            TodayCollection = todayCollection,
            MonthCollection = monthCollection,
            TotalOutstanding = chargeSummary?.Balance ?? 0m,
            DefaulterCount = defaulterCount,
            PendingDocuments = pendingDocuments
        };
    }

    public async Task<PagedResult<StudentMasterReportResponse>> GetStudentMasterAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = CurrentAdmissionQuery(tenantId, branchId, request);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                x.AdmissionNo.Contains(search) ||
                x.Student.FirstName.Contains(search) ||
                x.Student.LastName.Contains(search) ||
                (x.Student.MiddleName != null && x.Student.MiddleName.Contains(search)) ||
                (x.Student.Mobile != null && x.Student.Mobile.Contains(search)) ||
                (x.RollNo != null && x.RollNo.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<StudentStatus>(request.Status, true, out var studentStatus))
            query = query.Where(x => x.Student.Status == studentStatus);

        var resultQuery = query
            .OrderBy(x => x.AcademicClass.Sequence)
            .ThenBy(x => x.Section != null ? x.Section.Name : string.Empty)
            .ThenBy(x => x.RollNo)
            .ThenBy(x => x.Student.FirstName)
            .Select(x => new StudentMasterReportResponse
            {
                StudentId = x.StudentId,
                AdmissionId = x.Id,
                AdmissionNo = x.AdmissionNo,
                StudentName = (x.Student.FirstName + " " + (x.Student.MiddleName ?? string.Empty) + " " + x.Student.LastName).Trim(),
                Gender = x.Student.Gender.ToString(),
                DateOfBirth = x.Student.DateOfBirth,
                Mobile = x.Student.Mobile,
                Email = x.Student.Email,
                AcademicYearName = x.AcademicYear.Name,
                ClassName = x.AcademicClass.Name,
                SectionName = x.Section != null ? x.Section.Name : null,
                RollNo = x.RollNo,
                GuardianName = x.Student.Guardians.Where(g => g.IsPrimary).Select(g => g.Name).FirstOrDefault(),
                GuardianMobile = x.Student.Guardians.Where(g => g.IsPrimary).Select(g => g.Mobile).FirstOrDefault(),
                StudentStatus = x.Student.Status.ToString(),
                AdmissionStatus = x.Status.ToString()
            });

        return await resultQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    public async Task<PagedResult<AdmissionRegisterReportResponse>> GetAdmissionRegisterAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = AdmissionQuery(tenantId, branchId, request);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.AdmissionDate >= request.FromDate.Value.Date);

        if (request.ToDate.HasValue)
        {
            var toDate = request.ToDate.Value.Date.AddDays(1);
            query = query.Where(x => x.AdmissionDate < toDate);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                x.AdmissionNo.Contains(search) ||
                x.Student.FirstName.Contains(search) ||
                x.Student.LastName.Contains(search) ||
                (x.RollNo != null && x.RollNo.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<AdmissionStatus>(request.Status, true, out var admissionStatus))
            query = query.Where(x => x.Status == admissionStatus);

        var resultQuery = query
            .OrderByDescending(x => x.AdmissionDate)
            .ThenBy(x => x.Student.FirstName)
            .Select(x => new AdmissionRegisterReportResponse
            {
                AdmissionId = x.Id,
                StudentId = x.StudentId,
                AdmissionNo = x.AdmissionNo,
                AdmissionDate = x.AdmissionDate,
                StudentName = (x.Student.FirstName + " " + (x.Student.MiddleName ?? string.Empty) + " " + x.Student.LastName).Trim(),
                AcademicYearName = x.AcademicYear.Name,
                ClassName = x.AcademicClass.Name,
                SectionName = x.Section != null ? x.Section.Name : null,
                RollNo = x.RollNo,
                AdmissionStatus = x.Status.ToString(),
                IsCurrent = x.IsCurrent
            });

        return await resultQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    public async Task<PagedResult<FeeCollectionReportResponse>> GetFeeCollectionAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.FeeReceipts
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId && !x.IsCancelled);

        ApplyDateFilter(ref query, request);

        if (request.StudentId.HasValue)
            query = query.Where(x => x.StudentId == request.StudentId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                x.ReceiptNo.Contains(search) ||
                (x.Student != null && (x.Student.FirstName.Contains(search) || x.Student.LastName.Contains(search))) ||
                (x.StudentAdmission != null && x.StudentAdmission.AdmissionNo.Contains(search)));
        }

        var resultQuery = query
            .OrderByDescending(x => x.ReceiptDate)
            .ThenByDescending(x => x.Id)
            .Select(x => new FeeCollectionReportResponse
            {
                ReceiptId = x.Id,
                ReceiptNo = x.ReceiptNo,
                ReceiptDate = x.ReceiptDate,
                StudentId = x.StudentId,
                AdmissionId = x.StudentAdmissionId,
                AdmissionNo = x.StudentAdmission != null ? x.StudentAdmission.AdmissionNo : string.Empty,
                StudentName = x.Student != null ? (x.Student.FirstName + " " + (x.Student.MiddleName ?? string.Empty) + " " + x.Student.LastName).Trim() : string.Empty,
                AcademicYearName = x.StudentAdmission != null ? x.StudentAdmission.AcademicYear.Name : null,
                ClassName = x.StudentAdmission != null ? x.StudentAdmission.AcademicClass.Name : null,
                SectionName = x.StudentAdmission != null && x.StudentAdmission.Section != null ? x.StudentAdmission.Section.Name : null,
                PaymentMode = x.PaymentMode == 1 ? "Cash" : x.PaymentMode == 2 ? "UPI" : x.PaymentMode == 3 ? "Bank Transfer" : x.PaymentMode == 4 ? "Cheque" : x.PaymentMode == 5 ? "Card" : "Other",
                Amount = x.TotalAmount,
                TransactionReference = x.TransactionReference,
                Remarks = x.Remarks
            });

        return await resultQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    public async Task<PagedResult<OutstandingFeeReportResponse>> GetOutstandingFeesAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.StudentCharges
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId && !x.IsCancelled && x.StudentAdmissionId.HasValue && x.StudentId.HasValue);

        if (request.StudentId.HasValue)
            query = query.Where(x => x.StudentId == request.StudentId.Value);

        if (request.AcademicYearId.HasValue)
            query = query.Where(x => x.StudentAdmission.AcademicYearId == request.AcademicYearId.Value);

        if (request.ClassId.HasValue)
            query = query.Where(x => x.StudentAdmission.AcademicClassId == request.ClassId.Value);

        if (request.SectionId.HasValue)
            query = query.Where(x => x.StudentAdmission.SectionId == request.SectionId.Value);

        var resultQuery = query
            .GroupBy(x => new
            {
                StudentId = x.StudentId!.Value,
                AdmissionId = x.StudentAdmissionId!.Value,
                x.StudentAdmission.AdmissionNo,
                x.Student.FirstName,
                x.Student.MiddleName,
                x.Student.LastName,
                AcademicYearName = x.StudentAdmission.AcademicYear.Name,
                ClassName = x.StudentAdmission.AcademicClass.Name,
                SectionName = x.StudentAdmission.Section != null ? x.StudentAdmission.Section.Name : null,
                x.StudentAdmission.RollNo
            })
            .Select(g => new OutstandingFeeReportResponse
            {
                StudentId = g.Key.StudentId,
                AdmissionId = g.Key.AdmissionId,
                AdmissionNo = g.Key.AdmissionNo,
                StudentName = (g.Key.FirstName + " " + (g.Key.MiddleName ?? string.Empty) + " " + g.Key.LastName).Trim(),
                AcademicYearName = g.Key.AcademicYearName,
                ClassName = g.Key.ClassName,
                SectionName = g.Key.SectionName,
                RollNo = g.Key.RollNo,
                TotalAmount = g.Sum(x => x.Amount),
                TotalDiscount = g.Sum(x => x.DiscountAmount),
                TotalFine = g.Sum(x => x.FineAmount),
                NetAmount = g.Sum(x => x.Amount - x.DiscountAmount + x.FineAmount),
                TotalPaid = g.Sum(x => x.PaidAmount),
                Balance = g.Sum(x => x.Amount - x.DiscountAmount + x.FineAmount - x.PaidAmount)
            })
            .Where(x => x.Balance > 0)
            .OrderByDescending(x => x.Balance)
            .ThenBy(x => x.StudentName);

        return await resultQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    public async Task<PagedResult<StudentFeeLedgerReportResponse>> GetStudentFeeLedgerAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.StudentFeeLedgers
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId);

        if (request.StudentId.HasValue)
            query = query.Where(x => x.StudentId == request.StudentId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.EntryDate >= request.FromDate.Value.Date);

        if (request.ToDate.HasValue)
        {
            var toDate = request.ToDate.Value.Date.AddDays(1);
            query = query.Where(x => x.EntryDate < toDate);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                x.EntryType.Contains(search) ||
                (x.ReferenceNo != null && x.ReferenceNo.Contains(search)) ||
                (x.Remarks != null && x.Remarks.Contains(search)));
        }

        var resultQuery = query
            .OrderBy(x => x.EntryDate)
            .ThenBy(x => x.Id)
            .Select(x => new StudentFeeLedgerReportResponse
            {
                LedgerId = x.Id,
                StudentId = x.StudentId,
                AdmissionId = x.StudentAdmissionId,
                EntryDate = x.EntryDate,
                EntryType = x.EntryType,
                DebitAmount = x.DebitAmount,
                CreditAmount = x.CreditAmount,
                RunningBalance = x.RunningBalance,
                ReferenceNo = x.ReferenceNo,
                Remarks = x.Remarks
            });

        return await resultQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    public async Task<PagedResult<ReceiptRegisterReportResponse>> GetReceiptRegisterAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.FeeReceipts
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId);

        ApplyDateFilter(ref query, request);

        if (request.StudentId.HasValue)
            query = query.Where(x => x.StudentId == request.StudentId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (request.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.IsCancelled);
            else if (request.Status.Equals("active", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => !x.IsCancelled);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                x.ReceiptNo.Contains(search) ||
                (x.Student != null && (x.Student.FirstName.Contains(search) || x.Student.LastName.Contains(search))) ||
                (x.StudentAdmission != null && x.StudentAdmission.AdmissionNo.Contains(search)));
        }

        var resultQuery = query
            .OrderByDescending(x => x.ReceiptDate)
            .ThenByDescending(x => x.Id)
            .Select(x => new ReceiptRegisterReportResponse
            {
                ReceiptId = x.Id,
                ReceiptNo = x.ReceiptNo,
                ReceiptDate = x.ReceiptDate,
                StudentId = x.StudentId,
                AdmissionId = x.StudentAdmissionId,
                AdmissionNo = x.StudentAdmission != null ? x.StudentAdmission.AdmissionNo : string.Empty,
                StudentName = x.Student != null ? (x.Student.FirstName + " " + (x.Student.MiddleName ?? string.Empty) + " " + x.Student.LastName).Trim() : string.Empty,
                PaymentMode = x.PaymentMode == 1 ? "Cash" : x.PaymentMode == 2 ? "UPI" : x.PaymentMode == 3 ? "Bank Transfer" : x.PaymentMode == 4 ? "Cheque" : x.PaymentMode == 5 ? "Card" : "Other",
                TotalAmount = x.TotalAmount,
                IsCancelled = x.IsCancelled,
                CancelledOnUtc = x.CancelledOnUtc,
                CancelReason = x.CancelReason
            });

        return await resultQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    public async Task<PagedResult<AcademicStrengthReportResponse>> GetAcademicStrengthAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = CurrentAdmissionQuery(tenantId, branchId, request);

        var resultQuery = query
            .GroupBy(x => new
            {
                x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                ClassId = x.AcademicClassId,
                ClassName = x.AcademicClass.Name,
                ClassSequence = x.AcademicClass.Sequence,
                x.SectionId,
                SectionName = x.Section != null ? x.Section.Name : null
            })
            .OrderBy(x => x.Key.AcademicYearName)
            .ThenBy(x => x.Key.ClassSequence)
            .ThenBy(x => x.Key.SectionName)
            .Select(g => new AcademicStrengthReportResponse
            {
                AcademicYearId = g.Key.AcademicYearId,
                AcademicYearName = g.Key.AcademicYearName,
                ClassId = g.Key.ClassId,
                ClassName = g.Key.ClassName,
                SectionId = g.Key.SectionId,
                SectionName = g.Key.SectionName,
                StudentCount = g.Count()
            });

        return await resultQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    public async Task<PagedResult<PendingDocumentReportResponse>> GetPendingDocumentsAsync(
        int tenantId,
        int branchId,
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.StudentDocumentChecklists
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId && x.IsActive && x.DocumentModel.IsRequired);

        if (request.AcademicYearId.HasValue)
            query = query.Where(x => x.StudentAdmission.AcademicYearId == request.AcademicYearId.Value);

        if (request.ClassId.HasValue)
            query = query.Where(x => x.StudentAdmission.AcademicClassId == request.ClassId.Value);

        if (request.SectionId.HasValue)
            query = query.Where(x => x.StudentAdmission.SectionId == request.SectionId.Value);

        if (request.StudentId.HasValue)
            query = query.Where(x => x.StudentAdmission.StudentId == request.StudentId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                x.StudentAdmission.AdmissionNo.Contains(search) ||
                x.StudentAdmission.Student.FirstName.Contains(search) ||
                x.StudentAdmission.Student.LastName.Contains(search));
        }

        var resultQuery = query
            .GroupBy(x => new
            {
                AdmissionId = x.StudentAdmissionId,
                x.StudentAdmission.StudentId,
                x.StudentAdmission.AdmissionNo,
                x.StudentAdmission.Student.FirstName,
                x.StudentAdmission.Student.MiddleName,
                x.StudentAdmission.Student.LastName,
                AcademicYearName = x.StudentAdmission.AcademicYear.Name,
                ClassName = x.StudentAdmission.AcademicClass.Name,
                SectionName = x.StudentAdmission.Section != null ? x.StudentAdmission.Section.Name : null
            })
            .Select(g => new PendingDocumentReportResponse
            {
                AdmissionId = g.Key.AdmissionId,
                StudentId = g.Key.StudentId,
                AdmissionNo = g.Key.AdmissionNo,
                StudentName = (g.Key.FirstName + " " + (g.Key.MiddleName ?? string.Empty) + " " + g.Key.LastName).Trim(),
                AcademicYearName = g.Key.AcademicYearName,
                ClassName = g.Key.ClassName,
                SectionName = g.Key.SectionName,
                RequiredDocumentCount = g.Count(),
                ReceivedDocumentCount = g.Count(x => x.IsReceived),
                PendingDocumentCount = g.Count(x => !x.IsReceived)
            })
            .Where(x => x.PendingDocumentCount > 0)
            .OrderByDescending(x => x.PendingDocumentCount)
            .ThenBy(x => x.StudentName);

        return await resultQuery.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    private IQueryable<Shala.Domain.Entities.Students.StudentAdmission> AdmissionQuery(
        int tenantId,
        int branchId,
        ReportFilterRequest request)
    {
        var query = _db.StudentAdmissions
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId);

        if (request.AcademicYearId.HasValue)
            query = query.Where(x => x.AcademicYearId == request.AcademicYearId.Value);

        if (request.ClassId.HasValue)
            query = query.Where(x => x.AcademicClassId == request.ClassId.Value);

        if (request.SectionId.HasValue)
            query = query.Where(x => x.SectionId == request.SectionId.Value);

        if (request.StudentId.HasValue)
            query = query.Where(x => x.StudentId == request.StudentId.Value);

        return query;
    }

    private IQueryable<Shala.Domain.Entities.Students.StudentAdmission> CurrentAdmissionQuery(
        int tenantId,
        int branchId,
        ReportFilterRequest request)
    {
        return AdmissionQuery(tenantId, branchId, request)
            .Where(x => x.IsCurrent);
    }

    private static void ApplyDateFilter(ref IQueryable<Shala.Domain.Entities.Fees.FeeReceipt> query, ReportFilterRequest request)
    {
        if (request.FromDate.HasValue)
            query = query.Where(x => x.ReceiptDate >= request.FromDate.Value.Date);

        if (request.ToDate.HasValue)
        {
            var toDate = request.ToDate.Value.Date.AddDays(1);
            query = query.Where(x => x.ReceiptDate < toDate);
        }
    }
}
