using Shala.Shared.Requests.Supplies;
using Shala.Shared.Responses.Supplies;

namespace Shala.Web.Repositories.Supplies;

public interface ISuppliesWebRepository
{
    Task<List<SupplyItemResponse>> GetItemsAsync(bool activeOnly = false, CancellationToken cancellationToken = default);
    Task<SupplyItemResponse?> CreateItemAsync(CreateSupplyItemRequest request, CancellationToken cancellationToken = default);
    Task UpdateItemAsync(int id, UpdateSupplyItemRequest request, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(int id, CancellationToken cancellationToken = default);

    Task<SupplyDashboardResponse?> GetDashboardAsync(int? academicYearId = null, CancellationToken cancellationToken = default);
    Task AddStockAsync(AddSupplyStockRequest request, CancellationToken cancellationToken = default);

    Task<StudentSupplyIssueResponse?> CreateIssueAsync(CreateStudentSupplyIssueRequest request, CancellationToken cancellationToken = default);
    Task<List<StudentSupplyIssueResponse>> GetIssuesAsync(int? academicYearId = null, CancellationToken cancellationToken = default);
    Task<StudentSupplyIssueResponse?> GetIssueAsync(int id, CancellationToken cancellationToken = default);

    Task<List<PendingSupplyDueResponse>> GetDuesAsync(int? academicYearId = null, int? studentId = null, CancellationToken cancellationToken = default);
    Task<StudentSupplyIssueResponse?> CollectDueAsync(int issueId, CollectSupplyDueRequest request, CancellationToken cancellationToken = default);

    Task<List<SupplyItemResponse>> GetLowStockAsync(CancellationToken cancellationToken = default);
    Task<List<SupplyStockLedgerResponse>> GetStockHistoryAsync(SupplyReportRequest request, CancellationToken cancellationToken = default);
}