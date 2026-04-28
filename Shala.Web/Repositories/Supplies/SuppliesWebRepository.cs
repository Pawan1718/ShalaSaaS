using Shala.Shared.Requests.Supplies;
using Shala.Shared.Responses.Supplies;
using Shala.Web.Services.Http;

namespace Shala.Web.Repositories.Supplies;

public sealed class SuppliesWebRepository : ISuppliesWebRepository
{
    private readonly IHttpService _httpService;

    public SuppliesWebRepository(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<List<SupplyItemResponse>> GetItemsAsync(bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var res = await _httpService.GetAsync<List<SupplyItemResponse>>(
            $"api/tenant/supplies/items?activeOnly={activeOnly}",
            cancellationToken);

        EnsureSuccess(res);
        return res.ServerResponse ?? [];
    }

    public async Task<SupplyItemResponse?> CreateItemAsync(CreateSupplyItemRequest request, CancellationToken cancellationToken = default)
    {
        var res = await _httpService.PostAsync<CreateSupplyItemRequest, SupplyItemResponse>(
            "api/tenant/supplies/items",
            request,
            cancellationToken);

        EnsureSuccess(res);
        return res.ServerResponse;
    }

    public async Task UpdateItemAsync(int id, UpdateSupplyItemRequest request, CancellationToken cancellationToken = default)
    {
        var res = await _httpService.PutAsync<UpdateSupplyItemRequest, object>(
            $"api/tenant/supplies/items/{id}",
            request,
            cancellationToken);

        EnsureSuccess(res);
    }

    public async Task DeleteItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var res = await _httpService.DeleteAsync(
            $"api/tenant/supplies/items/{id}",
            cancellationToken);

        EnsureSuccess(res);
    }

    public async Task<SupplyDashboardResponse?> GetDashboardAsync(int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var url = academicYearId.HasValue
            ? $"api/tenant/supplies/dashboard?academicYearId={academicYearId.Value}"
            : "api/tenant/supplies/dashboard";

        var res = await _httpService.GetAsync<SupplyDashboardResponse>(url, cancellationToken);
        EnsureSuccess(res);
        return res.ServerResponse;
    }

    public async Task AddStockAsync(AddSupplyStockRequest request, CancellationToken cancellationToken = default)
    {
        var res = await _httpService.PostAsync<AddSupplyStockRequest, object>(
            "api/tenant/supplies/stock-in",
            request,
            cancellationToken);

        EnsureSuccess(res);
    }

    public async Task<StudentSupplyIssueResponse?> CreateIssueAsync(CreateStudentSupplyIssueRequest request, CancellationToken cancellationToken = default)
    {
        var res = await _httpService.PostAsync<CreateStudentSupplyIssueRequest, StudentSupplyIssueResponse>(
            "api/tenant/supplies/issues",
            request,
            cancellationToken);

        EnsureSuccess(res);
        return res.ServerResponse;
    }

    public async Task<List<StudentSupplyIssueResponse>> GetIssuesAsync(int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var url = academicYearId.HasValue
            ? $"api/tenant/supplies/issues?academicYearId={academicYearId.Value}"
            : "api/tenant/supplies/issues";

        var res = await _httpService.GetAsync<List<StudentSupplyIssueResponse>>(url, cancellationToken);
        EnsureSuccess(res);
        return res.ServerResponse ?? [];
    }

    public async Task<StudentSupplyIssueResponse?> GetIssueAsync(int id, CancellationToken cancellationToken = default)
    {
        var res = await _httpService.GetAsync<StudentSupplyIssueResponse>(
            $"api/tenant/supplies/issues/{id}",
            cancellationToken);

        EnsureSuccess(res);
        return res.ServerResponse;
    }

    public async Task<List<PendingSupplyDueResponse>> GetDuesAsync(int? academicYearId = null, int? studentId = null, CancellationToken cancellationToken = default)
    {
        var query = new List<string>();

        if (academicYearId.HasValue)
            query.Add($"academicYearId={academicYearId.Value}");

        if (studentId.HasValue)
            query.Add($"studentId={studentId.Value}");

        var url = "api/tenant/supplies/dues";
        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        var res = await _httpService.GetAsync<List<PendingSupplyDueResponse>>(url, cancellationToken);
        EnsureSuccess(res);
        return res.ServerResponse ?? [];
    }

    public async Task<StudentSupplyIssueResponse?> CollectDueAsync(int issueId, CollectSupplyDueRequest request, CancellationToken cancellationToken = default)
    {
        var res = await _httpService.PostAsync<CollectSupplyDueRequest, StudentSupplyIssueResponse>(
            $"api/tenant/supplies/issues/{issueId}/payments",
            request,
            cancellationToken);

        EnsureSuccess(res);
        return res.ServerResponse;
    }

    public async Task<List<SupplyItemResponse>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        var res = await _httpService.GetAsync<List<SupplyItemResponse>>(
            "api/tenant/supplies/reports/low-stock",
            cancellationToken);

        EnsureSuccess(res);
        return res.ServerResponse ?? [];
    }

    public async Task<List<SupplyStockLedgerResponse>> GetStockHistoryAsync(SupplyReportRequest request, CancellationToken cancellationToken = default)
    {
        var query = new List<string>();

        if (request.FromDate.HasValue)
            query.Add($"fromDate={request.FromDate.Value:yyyy-MM-dd}");

        if (request.ToDate.HasValue)
            query.Add($"toDate={request.ToDate.Value:yyyy-MM-dd}");

        if (request.SupplyItemId.HasValue)
            query.Add($"supplyItemId={request.SupplyItemId.Value}");

        var url = "api/tenant/supplies/reports/stock-history";
        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        var res = await _httpService.GetAsync<List<SupplyStockLedgerResponse>>(url, cancellationToken);
        EnsureSuccess(res);
        return res.ServerResponse ?? [];
    }

    private static void EnsureSuccess<T>(ServerResponseHelper<T> response)
    {
        if (!response.IsSuccess)
            throw new Exception(response.Message ?? "Request failed.");
    }
}