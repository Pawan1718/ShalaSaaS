using Shala.Shared.Common;
using Shala.Shared.Requests.Reports;
using Shala.Shared.Responses.Reports;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.Reports;

public sealed class ReportsWebRepository : IReportsWebRepository
{
    private readonly IHttpService _httpService;

    public ReportsWebRepository(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public Task<ServerResponseHelper<ReportsDashboardResponse>> GetDashboardAsync(
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, ReportsDashboardResponse>(
            "api/reports/dashboard",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<PagedResult<StudentMasterReportResponse>>> GetStudentMasterAsync(
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, PagedResult<StudentMasterReportResponse>>(
            "api/reports/students/master",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<PagedResult<AdmissionRegisterReportResponse>>> GetAdmissionRegisterAsync(
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, PagedResult<AdmissionRegisterReportResponse>>(
            "api/reports/admissions/register",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<PagedResult<FeeCollectionReportResponse>>> GetFeeCollectionAsync(
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, PagedResult<FeeCollectionReportResponse>>(
            "api/reports/fees/collection",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<PagedResult<OutstandingFeeReportResponse>>> GetOutstandingFeesAsync(
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, PagedResult<OutstandingFeeReportResponse>>(
            "api/reports/fees/outstanding",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<PagedResult<ReceiptRegisterReportResponse>>> GetReceiptRegisterAsync(
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, PagedResult<ReceiptRegisterReportResponse>>(
            "api/reports/fees/receipts",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<PagedResult<AcademicStrengthReportResponse>>> GetAcademicStrengthAsync(
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, PagedResult<AcademicStrengthReportResponse>>(
            "api/reports/academics/strength",
            request,
            cancellationToken);
    }

    public Task<ServerResponseHelper<PagedResult<PendingDocumentReportResponse>>> GetPendingDocumentsAsync(
        ReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, PagedResult<PendingDocumentReportResponse>>(
            "api/reports/documents/pending",
            request,
            cancellationToken);
    }
}
