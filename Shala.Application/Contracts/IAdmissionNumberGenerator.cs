namespace Shala.Application.Contracts;

public interface IAdmissionNumberGenerator
{
    Task<string> GenerateAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        CancellationToken cancellationToken = default);
}