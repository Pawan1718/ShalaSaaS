using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;

namespace Shala.Application.Features.Platform;

public interface IBranchService
{
    Task<(bool Success, BranchResponse? Data, string? Message)> CreateAsync(
      CreateBranchRequest request,
      CancellationToken cancellationToken = default);

    Task<(bool Success, BranchResponse? Data, string? Message)> GetByIdAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, List<BranchResponse>? Data, string? Message)> GetAllAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, BranchResponse? Data, string? Message)> UpdateAsync(
        int tenantId,
        int branchId,
        UpdateBranchRequest request,
        CancellationToken cancellationToken = default);

    Task<(bool Success, bool Data, string? Message)> DeleteAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);
}