using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shala.Shared.Common;
using Shala.Shared.Requests.Reports;
using Shala.Shared.Responses.Reports;
using Shala.Web.Services.Http;
using Shala.Web.Services.State;

namespace Shala.Web.Repositories.Reports;

public sealed class ReportsWebRepository : IReportsWebRepository
{
    private readonly IHttpService _httpService;
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;

    public ReportsWebRepository(IHttpService httpService, HttpClient httpClient, ApiSession session)
    {
        _httpService = httpService;
        _httpClient = httpClient;
        _session = session;
    }

    public Task<ServerResponseHelper<ReportsDashboardResponse>> GetDashboardAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, ReportsDashboardResponse>("api/reports/dashboard", request, cancellationToken);

    public Task<ServerResponseHelper<PagedResult<StudentMasterReportResponse>>> GetStudentMasterAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, PagedResult<StudentMasterReportResponse>>("api/reports/students/master", request, cancellationToken);

    public Task<ServerResponseHelper<PagedResult<AdmissionRegisterReportResponse>>> GetAdmissionRegisterAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, PagedResult<AdmissionRegisterReportResponse>>("api/reports/admissions/register", request, cancellationToken);

    public Task<ServerResponseHelper<PagedResult<FeeCollectionReportResponse>>> GetFeeCollectionAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, PagedResult<FeeCollectionReportResponse>>("api/reports/fees/collection", request, cancellationToken);

    public Task<ServerResponseHelper<PagedResult<OutstandingFeeReportResponse>>> GetOutstandingFeesAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, PagedResult<OutstandingFeeReportResponse>>("api/reports/fees/outstanding", request, cancellationToken);

    public Task<ServerResponseHelper<PagedResult<StudentFeeLedgerReportResponse>>> GetStudentFeeLedgerAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, PagedResult<StudentFeeLedgerReportResponse>>("api/reports/fees/student-ledger", request, cancellationToken);

    public Task<ServerResponseHelper<PagedResult<ReceiptRegisterReportResponse>>> GetReceiptRegisterAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, PagedResult<ReceiptRegisterReportResponse>>("api/reports/fees/receipts", request, cancellationToken);

    public Task<ServerResponseHelper<PagedResult<AcademicStrengthReportResponse>>> GetAcademicStrengthAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, PagedResult<AcademicStrengthReportResponse>>("api/reports/academics/strength", request, cancellationToken);

    public Task<ServerResponseHelper<PagedResult<PendingDocumentReportResponse>>> GetPendingDocumentsAsync(ReportFilterRequest request, CancellationToken cancellationToken = default)
        => _httpService.PostAsync<ReportFilterRequest, PagedResult<PendingDocumentReportResponse>>("api/reports/documents/pending", request, cancellationToken);


    public Task<ServerResponseHelper<PagedResult<StudentDobAgeReportResponse>>> GetStudentDobAgeReportAsync(
    ReportFilterRequest request,
    CancellationToken cancellationToken = default)
    {
        return _httpService.PostAsync<ReportFilterRequest, PagedResult<StudentDobAgeReportResponse>>(
            "api/reports/students/dob-age",
            request,
            cancellationToken);
    }

    public async Task<ReportDownloadResult> ExportAsync(ReportExportRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_session.IsAuthenticated)
                await _session.InitializeAsync();

            _httpClient.DefaultRequestHeaders.Authorization = null;
            _httpClient.DefaultRequestHeaders.Remove("X-Branch-Id");
            _httpClient.DefaultRequestHeaders.Remove("X-All-Branches");

            if (!string.IsNullOrWhiteSpace(_session.Token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _session.Token);

            if (_session.IsAllBranchesSelected)
                _httpClient.DefaultRequestHeaders.Add("X-All-Branches", "true");
            else if (_session.SelectedBranchId.HasValue)
                _httpClient.DefaultRequestHeaders.Add("X-Branch-Id", _session.SelectedBranchId.Value.ToString());

            using var response = await _httpClient.PostAsJsonAsync("api/reports/export", request, cancellationToken);
            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = bytes.Length > 0 ? System.Text.Encoding.UTF8.GetString(bytes) : "Unable to export report.";
                return new ReportDownloadResult { IsSuccess = false, Message = message };
            }

            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                ?? $"{request.ReportKey}.csv";

            return new ReportDownloadResult
            {
                IsSuccess = true,
                Content = bytes,
                FileName = fileName,
                ContentType = response.Content.Headers.ContentType?.MediaType ?? "text/csv"
            };
        }
        catch (Exception ex)
        {
            return new ReportDownloadResult { IsSuccess = false, Message = ex.Message };
        }
    }
}
