using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Reports;
using Shala.Shared.Common;
using Shala.Shared.Requests.Reports;
using Shala.Shared.Responses.Reports;

namespace Shala.Api.Controllers;

[Route("api/reports")]
public sealed class ReportsController : TenantApiControllerBase
{
    private readonly IReportsService _reportsService;

    public ReportsController(
        IReportsService reportsService,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _reportsService = reportsService;
    }

    [HttpPost("dashboard")]
    public async Task<IActionResult> GetDashboard([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetDashboardAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<ReportsDashboardResponse>.Ok(result, "Report loaded successfully."));
    }

    [HttpPost("students/master")]
    public async Task<IActionResult> GetStudentMaster([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetStudentMasterAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<PagedResult<StudentMasterReportResponse>>.Ok(result, "Report loaded successfully."));
    }

    [HttpPost("admissions/register")]
    public async Task<IActionResult> GetAdmissionRegister([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetAdmissionRegisterAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<PagedResult<AdmissionRegisterReportResponse>>.Ok(result, "Report loaded successfully."));
    }

    [HttpPost("fees/collection")]
    public async Task<IActionResult> GetFeeCollection([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetFeeCollectionAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<PagedResult<FeeCollectionReportResponse>>.Ok(result, "Report loaded successfully."));
    }

    [HttpPost("fees/outstanding")]
    public async Task<IActionResult> GetOutstandingFees([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetOutstandingFeesAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<PagedResult<OutstandingFeeReportResponse>>.Ok(result, "Report loaded successfully."));
    }

    [HttpPost("fees/student-ledger")]
    public async Task<IActionResult> GetStudentFeeLedger([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetStudentFeeLedgerAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<PagedResult<StudentFeeLedgerReportResponse>>.Ok(result, "Report loaded successfully."));
    }

    [HttpPost("fees/receipts")]
    public async Task<IActionResult> GetReceiptRegister([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetReceiptRegisterAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<PagedResult<ReceiptRegisterReportResponse>>.Ok(result, "Report loaded successfully."));
    }

    [HttpPost("academics/strength")]
    public async Task<IActionResult> GetAcademicStrength([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetAcademicStrengthAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<PagedResult<AcademicStrengthReportResponse>>.Ok(result, "Report loaded successfully."));
    }

    [HttpPost("documents/pending")]
    public async Task<IActionResult> GetPendingDocuments([FromBody] ReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetPendingDocumentsAsync(TenantId, BranchId, request, cancellationToken);
        return Ok(ApiResponse<PagedResult<PendingDocumentReportResponse>>.Ok(result, "Report loaded successfully."));
    }


    [HttpPost("students/dob-age")]
    public async Task<IActionResult> GetStudentDobAgeReport(
    [FromBody] ReportFilterRequest request,
    CancellationToken cancellationToken)
    {
        var result = await _reportsService.GetStudentDobAgeReportAsync(
            TenantId,
            BranchId,
            request,
            cancellationToken);

        return Ok(ApiResponse<PagedResult<StudentDobAgeReportResponse>>.Ok(
            result,
            "Report loaded successfully."));
    }


    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] ReportExportRequest request, CancellationToken cancellationToken)
    {
        if (!request.ExportType.Equals("excel", StringComparison.OrdinalIgnoreCase))
            return BadRequest(ApiResponse<object>.Fail("Only Excel export is supported from API. Use browser print for PDF."));

        request.PageNumber = 1;
        request.PageSize = -1;

        var filter = request.ToFilterRequest();

        var csv = request.ReportKey.Trim().ToLowerInvariant() switch
        {
            "student-master" => ToCsv(
                (await _reportsService.GetStudentMasterAsync(TenantId, BranchId, filter, cancellationToken)).Items,
                ("Admission No", x => x.AdmissionNo),
                ("Student", x => x.StudentName),
                ("Class", x => x.ClassName ?? string.Empty),
                ("Section", x => x.SectionName ?? string.Empty),
                ("Roll No", x => x.RollNo ?? string.Empty),
                ("Mobile", x => x.Mobile ?? string.Empty),
                ("Guardian", x => x.GuardianName ?? string.Empty),
                ("Status", x => x.StudentStatus ?? string.Empty)),

            "admission-register" => ToCsv(
                (await _reportsService.GetAdmissionRegisterAsync(TenantId, BranchId, filter, cancellationToken)).Items,
                ("Date", x => x.AdmissionDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)),
                ("Admission No", x => x.AdmissionNo),
                ("Student", x => x.StudentName),
                ("Academic Year", x => x.AcademicYearName ?? string.Empty),
                ("Class", x => x.ClassName ?? string.Empty),
                ("Section", x => x.SectionName ?? string.Empty),
                ("Roll No", x => x.RollNo ?? string.Empty),
                ("Status", x => x.AdmissionStatus ?? string.Empty)),

            "fee-collection" => ToCsv(
                (await _reportsService.GetFeeCollectionAsync(TenantId, BranchId, filter, cancellationToken)).Items,
                ("Receipt No", x => x.ReceiptNo),
                ("Date", x => x.ReceiptDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)),
                ("Student", x => x.StudentName),
                ("Admission No", x => x.AdmissionNo),
                ("Class", x => x.ClassName ?? string.Empty),
                ("Section", x => x.SectionName ?? string.Empty),
                ("Payment Mode", x => x.PaymentMode),
                ("Amount", x => x.Amount.ToString("0.00", CultureInfo.InvariantCulture))),

            "outstanding-fees" => ToCsv(
                (await _reportsService.GetOutstandingFeesAsync(TenantId, BranchId, filter, cancellationToken)).Items,
                ("Admission No", x => x.AdmissionNo),
                ("Student", x => x.StudentName),
                ("Class", x => x.ClassName ?? string.Empty),
                ("Section", x => x.SectionName ?? string.Empty),
                ("Net Amount", x => x.NetAmount.ToString("0.00", CultureInfo.InvariantCulture)),
                ("Paid", x => x.TotalPaid.ToString("0.00", CultureInfo.InvariantCulture)),
                ("Balance", x => x.Balance.ToString("0.00", CultureInfo.InvariantCulture))),

            "student-ledger" => ToCsv(
                (await _reportsService.GetStudentFeeLedgerAsync(TenantId, BranchId, filter, cancellationToken)).Items,
                ("Date", x => x.EntryDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)),
                ("Type", x => x.EntryType),
                ("Debit", x => x.DebitAmount.ToString("0.00", CultureInfo.InvariantCulture)),
                ("Credit", x => x.CreditAmount.ToString("0.00", CultureInfo.InvariantCulture)),
                ("Balance", x => x.RunningBalance.ToString("0.00", CultureInfo.InvariantCulture)),
                ("Reference", x => x.ReferenceNo ?? string.Empty),
                ("Remarks", x => x.Remarks ?? string.Empty)),

            "receipt-register" => ToCsv(
                (await _reportsService.GetReceiptRegisterAsync(TenantId, BranchId, filter, cancellationToken)).Items,
                ("Receipt No", x => x.ReceiptNo),
                ("Date", x => x.ReceiptDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)),
                ("Student", x => x.StudentName),
                ("Admission No", x => x.AdmissionNo),
                ("Payment Mode", x => x.PaymentMode),
                ("Amount", x => x.TotalAmount.ToString("0.00", CultureInfo.InvariantCulture)),
                ("Status", x => x.IsCancelled ? "Cancelled" : "Active")),

            "academic-strength" => ToCsv(
                (await _reportsService.GetAcademicStrengthAsync(TenantId, BranchId, filter, cancellationToken)).Items,
                ("Academic Year", x => x.AcademicYearName ?? string.Empty),
                ("Class", x => x.ClassName),
                ("Section", x => x.SectionName ?? string.Empty),
                ("Students", x => x.StudentCount.ToString(CultureInfo.InvariantCulture))),

            "pending-documents" => ToCsv(
                (await _reportsService.GetPendingDocumentsAsync(TenantId, BranchId, filter, cancellationToken)).Items,
                ("Admission No", x => x.AdmissionNo),
                ("Student", x => x.StudentName),
                ("Academic Year", x => x.AcademicYearName ?? string.Empty),
                ("Class", x => x.ClassName ?? string.Empty),
                ("Section", x => x.SectionName ?? string.Empty),
                ("Required", x => x.RequiredDocumentCount.ToString(CultureInfo.InvariantCulture)),
                ("Received", x => x.ReceivedDocumentCount.ToString(CultureInfo.InvariantCulture)),
                ("Pending", x => x.PendingDocumentCount.ToString(CultureInfo.InvariantCulture))),

            "student-dob-age" => ToCsv(
(await _reportsService.GetStudentDobAgeReportAsync(TenantId, BranchId, filter, cancellationToken)).Items,
("Admission No", x => x.AdmissionNo),
("Student", x => x.StudentName),
("Class", x => x.ClassName ?? string.Empty),
("Section", x => x.SectionName ?? string.Empty),
("Roll No", x => x.RollNo ?? string.Empty),
("DOB", x => x.DateOfBirth?.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) ?? string.Empty),
("Age", x => x.AgeText)),
            _ => null
        };

        if (csv is null)
            return BadRequest(ApiResponse<object>.Fail("Invalid report key."));

        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv))
            .ToArray();

        var fileName = $"{request.ReportKey}-{DateTime.Now:yyyyMMdd-HHmm}.csv";

        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    private static string ToCsv<T>(
        IEnumerable<T> rows,
        params (string Header, Func<T, string> Value)[] columns)
    {
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(',', columns.Select(c => Escape(c.Header))));

        foreach (var row in rows)
        {
            sb.AppendLine(string.Join(',', columns.Select(c => Escape(c.Value(row)))));
        }

        return sb.ToString();
    }

    private static string Escape(string? value)
    {
        value ??= string.Empty;
        value = value.Replace("\r", " ").Replace("\n", " ").Trim();

        return value.Contains(',') || value.Contains('"')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }
}