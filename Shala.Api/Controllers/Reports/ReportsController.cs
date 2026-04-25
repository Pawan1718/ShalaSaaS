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
}