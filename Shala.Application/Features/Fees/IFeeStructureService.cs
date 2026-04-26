using Shala.Domain.Entities.Fees;

namespace Shala.Application.Features.Fees;

public interface IFeeStructureService
{
    Task<List<FeeStructure>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<FeeStructure?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<FeeStructure?> GetWithItemsAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message, FeeStructure? Data)> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        FeeStructure entity,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        FeeStructure entity,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default);
}