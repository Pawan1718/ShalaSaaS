using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;

namespace Shala.Application.Features.Fees;

public class StudentChargeService : IStudentChargeService
{
    private readonly IStudentChargeRepository _repo;
    private readonly IFeeLedgerWriteRepository _ledgerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentChargeService(
        IStudentChargeRepository repo,
        IFeeLedgerWriteRepository ledgerRepository,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _ledgerRepository = ledgerRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<List<StudentCharge>> GetByStudentIdAsync(
        int tenantId,
        int branchId,
        int studentId,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetByStudentIdAsync(studentId, tenantId, branchId, cancellationToken);
    }

    public Task<List<StudentCharge>> GetByAssignmentIdAsync(
        int tenantId,
        int branchId,
        int studentFeeAssignmentId,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetByAssignmentIdAsync(studentFeeAssignmentId, tenantId, branchId, cancellationToken);
    }

    public Task<StudentCharge?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
    }

    public async Task<(bool Success, string Message)> AddRangeAsync(
        List<StudentCharge> entities,
        CancellationToken cancellationToken = default)
    {
        if (entities == null || entities.Count == 0)
            return (false, "No student charges to add.");

        if (entities.Any(x => x.Amount <= 0))
            return (false, "Charge amount must be greater than zero.");

        await _repo.AddRangeAsync(entities, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var admissionIds = entities
            .Where(x => x.StudentAdmissionId.HasValue && x.StudentAdmissionId.Value > 0)
            .Select(x => x.StudentAdmissionId!.Value)
            .Distinct()
            .ToList();

        if (admissionIds.Count > 0)
        {
            var tenantId = entities.First().TenantId;
            var branchId = entities.First().BranchId;

            foreach (var admissionId in admissionIds)
            {
                await _ledgerRepository.RebuildRunningBalanceAsync(
                    tenantId,
                    branchId,
                    admissionId,
                    cancellationToken);
            }

            await _ledgerRepository.SaveChangesAsync(cancellationToken);
        }

        return (true, "Student charges created successfully.");
    }

    public async Task<(bool Success, string Message)> MarkCancelledAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
        if (existing is null)
            return (false, "Student charge not found.");

        if (existing.IsCancelled)
            return (false, "Student charge is already cancelled.");

        if (existing.PaidAmount > 0)
            return (false, "Paid charge cannot be cancelled.");

        if (existing.Allocations != null && existing.Allocations.Count > 0)
            return (false, "Charge with receipt allocations cannot be cancelled.");

        existing.IsCancelled = true;
        existing.IsSettled = false;

        _repo.Update(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (existing.StudentAdmissionId.HasValue && existing.StudentAdmissionId.Value > 0)
        {
            await _ledgerRepository.RebuildRunningBalanceAsync(
                tenantId,
                branchId,
                existing.StudentAdmissionId.Value,
                cancellationToken);

            await _ledgerRepository.SaveChangesAsync(cancellationToken);
        }

        return (true, "Student charge cancelled successfully.");
    }
}