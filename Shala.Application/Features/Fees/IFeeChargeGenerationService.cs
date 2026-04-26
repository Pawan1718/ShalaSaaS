using Shala.Domain.Entities.Fees;

namespace Shala.Application.Features.Fees;

public interface IFeeChargeGenerationService
{
    Task<(bool Success, string Message, List<StudentCharge> Charges)> GenerateAsync(
        int tenantId,
        int branchId,
        int studentFeeAssignmentId,
        CancellationToken cancellationToken = default);


}