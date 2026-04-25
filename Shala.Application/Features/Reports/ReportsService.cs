using Shala.Application.Repositories.Reports;
using Shala.Shared.Common;
using Shala.Shared.Requests.Reports;
using Shala.Shared.Responses.Reports;

namespace Shala.Application.Features.Reports;

public sealed class ReportsService : IReportsService
{
    private readonly IReportsRepository _reportsRepository;

    public ReportsService(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public Task<ReportsDashboardResponse> GetDashboardAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetDashboardAsync(tenantId, branchId, request, cancellationToken);
    }

    public Task<PagedResult<StudentMasterReportResponse>> GetStudentMasterAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetStudentMasterAsync(tenantId, branchId, request, cancellationToken);
    }

    public Task<PagedResult<AdmissionRegisterReportResponse>> GetAdmissionRegisterAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetAdmissionRegisterAsync(tenantId, branchId, request, cancellationToken);
    }

    public Task<PagedResult<FeeCollectionReportResponse>> GetFeeCollectionAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetFeeCollectionAsync(tenantId, branchId, request, cancellationToken);
    }

    public Task<PagedResult<OutstandingFeeReportResponse>> GetOutstandingFeesAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetOutstandingFeesAsync(tenantId, branchId, request, cancellationToken);
    }

    public Task<PagedResult<StudentFeeLedgerReportResponse>> GetStudentFeeLedgerAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetStudentFeeLedgerAsync(tenantId, branchId, request, cancellationToken);
    }

    public Task<PagedResult<ReceiptRegisterReportResponse>> GetReceiptRegisterAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetReceiptRegisterAsync(tenantId, branchId, request, cancellationToken);
    }

    public Task<PagedResult<AcademicStrengthReportResponse>> GetAcademicStrengthAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetAcademicStrengthAsync(tenantId, branchId, request, cancellationToken);
    }

    public Task<PagedResult<PendingDocumentReportResponse>> GetPendingDocumentsAsync(int tenantId, int branchId, ReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        Normalize(request);
        return _reportsRepository.GetPendingDocumentsAsync(tenantId, branchId, request, cancellationToken);
    }

    private static void Normalize(ReportFilterRequest request)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

        if (request.PageSize == 0 || request.PageSize < -1)
            request.PageSize = 10;

        request.SearchText = request.SearchText?.Trim();
        request.SortBy = request.SortBy?.Trim();
        request.Status = request.Status?.Trim();
    }
}
