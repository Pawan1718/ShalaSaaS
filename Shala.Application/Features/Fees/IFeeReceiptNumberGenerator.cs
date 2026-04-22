namespace Shala.Application.Features.Fees;

public interface IFeeReceiptNumberGenerator
{
    Task<string> GenerateAsync(int tenantId, int branchId, CancellationToken cancellationToken = default);
}