using Shala.Shared.Common;
using Shala.Shared.Requests.Reports;
using Shala.Shared.Responses.Reports;

namespace Shala.Application.Features.Reports;

public interface IReportsService
{
    Task<ReportsDashboardResponse> GetDashboardAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<StudentMasterReportResponse>> GetStudentMasterAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<AdmissionRegisterReportResponse>> GetAdmissionRegisterAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<FeeCollectionReportResponse>> GetFeeCollectionAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<OutstandingFeeReportResponse>> GetOutstandingFeesAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<StudentFeeLedgerReportResponse>> GetStudentFeeLedgerAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<ReceiptRegisterReportResponse>> GetReceiptRegisterAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<AcademicStrengthReportResponse>> GetAcademicStrengthAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<PendingDocumentReportResponse>> GetPendingDocumentsAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<StudentDobAgeReportResponse>> GetStudentDobAgeReportAsync(
    int tenantId,
    int branchId,
    ReportFilterRequest request,
    CancellationToken cancellationToken = default);
}
