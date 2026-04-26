using Shala.Shared.Requests.Supplies;
using Shala.Shared.Responses.Supplies;

namespace Shala.Application.Features.Supplies;

public interface IStudentSupplyService
{
    Task<List<SupplyItemResponse>> GetItemsAsync(int tenantId, int branchId, bool activeOnly = false, CancellationToken cancellationToken = default);

    Task<(bool Success, string Message, SupplyItemResponse? Data)> CreateItemAsync(int tenantId, int branchId, string actor, CreateSupplyItemRequest request, CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> UpdateItemAsync(int tenantId, int branchId, string actor, int id, UpdateSupplyItemRequest request, CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> DeleteItemAsync(int tenantId, int branchId, int id, CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> AddStockAsync(int tenantId, int branchId, string actor, AddSupplyStockRequest request, CancellationToken cancellationToken = default);

    Task<(bool Success, string Message, StudentSupplyIssueResponse? Data)> CreateIssueAsync(int tenantId, int branchId, string actor, CreateStudentSupplyIssueRequest request, CancellationToken cancellationToken = default);

    Task<StudentSupplyIssueResponse?> GetIssueDetailAsync(int tenantId, int branchId, int id, CancellationToken cancellationToken = default);

    Task<List<StudentSupplyIssueResponse>> GetRecentIssuesAsync(int tenantId, int branchId, int? academicYearId = null, CancellationToken cancellationToken = default);

    Task<List<StudentSupplyIssueResponse>> GetStudentHistoryAsync(int tenantId, int branchId, int studentId, int? academicYearId = null, CancellationToken cancellationToken = default);

    Task<List<PendingSupplyDueResponse>> GetPendingDuesAsync(int tenantId, int branchId, int? academicYearId = null, int? studentId = null, CancellationToken cancellationToken = default);

    Task<(bool Success, string Message, StudentSupplyIssueResponse? Data)> CollectDueAsync(int tenantId, int branchId, string actor, int issueId, CollectSupplyDueRequest request, CancellationToken cancellationToken = default);

    Task<SupplyDashboardResponse> GetDashboardAsync(int tenantId, int branchId, int? academicYearId = null, CancellationToken cancellationToken = default);

    Task<List<SupplyItemResponse>> GetLowStockAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<List<SupplyStockLedgerResponse>> GetStockHistoryAsync(int tenantId, int branchId, SupplyReportRequest request, CancellationToken cancellationToken = default);
}