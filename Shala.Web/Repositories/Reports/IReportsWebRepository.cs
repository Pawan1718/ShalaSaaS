using Shala.Shared.Common;
using Shala.Shared.Requests.Reports;
using Shala.Shared.Responses.Reports;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.Reports;

public interface IReportsWebRepository
{
    Task<ServerResponseHelper<ReportsDashboardResponse>> GetDashboardAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<ServerResponseHelper<PagedResult<StudentMasterReportResponse>>> GetStudentMasterAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<ServerResponseHelper<PagedResult<AdmissionRegisterReportResponse>>> GetAdmissionRegisterAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<ServerResponseHelper<PagedResult<FeeCollectionReportResponse>>> GetFeeCollectionAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<ServerResponseHelper<PagedResult<OutstandingFeeReportResponse>>> GetOutstandingFeesAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<ServerResponseHelper<PagedResult<StudentFeeLedgerReportResponse>>> GetStudentFeeLedgerAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<ServerResponseHelper<PagedResult<ReceiptRegisterReportResponse>>> GetReceiptRegisterAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<ServerResponseHelper<PagedResult<AcademicStrengthReportResponse>>> GetAcademicStrengthAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<ServerResponseHelper<PagedResult<PendingDocumentReportResponse>>> GetPendingDocumentsAsync(ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<ServerResponseHelper<PagedResult<StudentDobAgeReportResponse>>> GetStudentDobAgeReportAsync(
    ReportFilterRequest request,
    CancellationToken cancellationToken = default);

    Task<ReportDownloadResult> ExportAsync(ReportExportRequest request, CancellationToken cancellationToken = default);
}
