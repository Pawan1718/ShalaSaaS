using Shala.Domain.Entities.Students;

namespace Shala.Application.Repositories.Students;

public interface IStudentGuardianRepository
{
    Task<Guardian?> GetGuardianByIdAsync(
        int guardianId,
        int studentId,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<int> GetGuardianCountAsync(
        int studentId,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task AddGuardianAsync(Guardian guardian, CancellationToken cancellationToken = default);
    void UpdateGuardian(Guardian guardian);
    void DeleteGuardian(Guardian guardian);
}