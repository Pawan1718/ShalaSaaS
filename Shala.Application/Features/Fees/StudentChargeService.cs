using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Common;
using Shala.Shared.Responses.Fees;
using System.Data;

namespace Shala.Application.Features.Fees;

public class StudentChargeService : IStudentChargeService
{
    private readonly IStudentChargeRepository _repo;
    private readonly IFeeLedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public StudentChargeService(
        IStudentChargeRepository repo,
        IFeeLedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _ledgerPostingService = ledgerPostingService;
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

        await _unitOfWork.BeginTransactionAsync(cancellationToken, IsolationLevel.Serializable);

        try
        {
            await _repo.AddRangeAsync(entities, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var groupedAdmissions = entities
                .Where(x => x.StudentAdmissionId.HasValue && x.StudentAdmissionId.Value > 0)
                .GroupBy(x => new
                {
                    x.TenantId,
                    x.BranchId,
                    x.StudentId,
                    StudentAdmissionId = x.StudentAdmissionId!.Value
                })
                .ToList();

            foreach (var group in groupedAdmissions)
            {
                await _ledgerPostingService.PostChargesAsync(
                    group.Key.TenantId,
                    group.Key.BranchId,
                    group.Key.StudentId ?? 0,
                    group.Key.StudentAdmissionId,
                    group.Select(ToChargeResponse),
                    cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return (true, "Student charges created successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
    public async Task<PagedResult<StudentChargeResponse>> GetPagedByStudentIdAsync(
       int tenantId,
       int branchId,
       int studentId,
       int pageNumber,
       int pageSize,
       CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var (items, totalCount) = await _repo.GetPagedByStudentIdAsync(
            studentId,
            tenantId,
            branchId,
            pageNumber,
            pageSize,
            cancellationToken);

        return new PagedResult<StudentChargeResponse>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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

        await _unitOfWork.BeginTransactionAsync(cancellationToken, IsolationLevel.Serializable);

        try
        {
            existing.IsCancelled = true;
            existing.IsSettled = false;
            _repo.Update(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (existing.StudentAdmissionId.HasValue && existing.StudentAdmissionId.Value > 0)
            {
                await _ledgerPostingService.PostChargesAsync(
                    tenantId,
                    branchId,
                    existing.StudentId ?? 0,
                    existing.StudentAdmissionId.Value,
                    new[] { ToChargeResponse(existing) },
                    cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return (true, "Student charge cancelled successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static StudentChargeResponse ToChargeResponse(StudentCharge charge)
    {
        return new StudentChargeResponse
        {
            Id = charge.Id,
            FeeHeadId = charge.FeeHeadId,
            Amount = charge.Amount,
            DiscountAmount = charge.DiscountAmount,
            FineAmount = charge.FineAmount,
            PaidAmount = charge.PaidAmount,
            DueDate = charge.DueDate,
            IsCancelled = charge.IsCancelled
        };
    }
}