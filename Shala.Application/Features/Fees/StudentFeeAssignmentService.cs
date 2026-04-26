using System.Data;
using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public class StudentFeeAssignmentService : IStudentFeeAssignmentService
{
    private readonly IStudentFeeAssignmentRepository _repo;
    private readonly IStudentChargeRepository _chargeRepository;
    private readonly IFeeLedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public StudentFeeAssignmentService(
        IStudentFeeAssignmentRepository repo,
        IStudentChargeRepository chargeRepository,
        IFeeLedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _chargeRepository = chargeRepository;
        _ledgerPostingService = ledgerPostingService;
        _unitOfWork = unitOfWork;
    }

    public Task<List<StudentFeeAssignment>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetAllAsync(tenantId, branchId, cancellationToken);
    }

    public Task<StudentFeeAssignment?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
    }

    public Task<StudentFeeAssignment?> GetByAdmissionIdAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default)
    {
        return _repo.GetByAdmissionIdAsync(studentAdmissionId, tenantId, branchId, cancellationToken);
    }

    public async Task<(bool Success, string Message, StudentFeeAssignment? Data)> AssignAsync(
        int tenantId,
        int branchId,
        StudentFeeAssignment entity,
        CancellationToken cancellationToken = default)
    {
        if (entity.StudentAdmissionId <= 0)
            return (false, "Student admission is required.", null);

        if (entity.StudentId <= 0)
            return (false, "Student is required.", null);

        if (entity.FeeStructureId <= 0)
            return (false, "Fee structure is required.", null);

        entity.TenantId = tenantId;
        entity.BranchId = branchId;
        entity.IsActive = true;

        var existing = await _repo.GetByAdmissionIdAsync(entity.StudentAdmissionId, tenantId, branchId, cancellationToken);
        if (existing is not null)
            return (false, "Fee structure already assigned for this admission.", null);

        await _repo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var saved = await _repo.GetByIdAsync(entity.Id, tenantId, branchId, cancellationToken);
        return (true, "Fee structure assigned successfully.", saved ?? entity);
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        int tenantId,
        int branchId,
        StudentFeeAssignment entity,
        CancellationToken cancellationToken = default)
    {
        if (entity.Id <= 0)
            return (false, "Assignment id is required.");

        if (entity.FeeStructureId <= 0)
            return (false, "Fee structure is required.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken, IsolationLevel.Serializable);

        try
        {
            var existing = await _repo.GetByIdAsync(entity.Id, tenantId, branchId, cancellationToken);
            if (existing is null)
                return await RollbackAsync("Student fee assignment not found.", cancellationToken);

            var existingCharges = await _chargeRepository.GetByAssignmentIdAsync(existing.Id, tenantId, branchId, cancellationToken);
            var hasPaidCharges = existingCharges.Any(x => x.PaidAmount > 0);

            var feeImpactChanged =
                existing.FeeStructureId != entity.FeeStructureId ||
                existing.DiscountAmount != entity.DiscountAmount ||
                existing.AdditionalChargeAmount != entity.AdditionalChargeAmount;

            if (hasPaidCharges && feeImpactChanged)
                return await RollbackAsync("Cannot change fee structure or fee amounts because some charges are already paid.", cancellationToken);

            existing.FeeStructureId = entity.FeeStructureId;
            existing.DiscountAmount = entity.DiscountAmount;
            existing.AdditionalChargeAmount = entity.AdditionalChargeAmount;
            existing.IsActive = entity.IsActive;

            var cancelledCharges = new List<StudentCharge>();
            if (feeImpactChanged)
            {
                foreach (var charge in existingCharges.Where(x => !x.IsCancelled && x.PaidAmount <= 0))
                {
                    charge.IsCancelled = true;
                    charge.IsSettled = false;
                    _chargeRepository.Update(charge);
                    cancelledCharges.Add(charge);
                }
            }

            _repo.Update(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (cancelledCharges.Count > 0)
            {
                await _ledgerPostingService.PostChargesAsync(
                    tenantId,
                    branchId,
                    existing.StudentId,
                    existing.StudentAdmissionId,
                    cancelledCharges.Select(ToChargeResponse),
                    cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return feeImpactChanged
                ? (true, "Student fee assignment updated successfully. Existing unpaid charges were cancelled. Generate charges again.")
                : (true, "Student fee assignment updated successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return (false, "Assignment id is required.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken, IsolationLevel.Serializable);

        try
        {
            var existing = await _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
            if (existing is null)
                return await RollbackAsync("Student fee assignment not found.", cancellationToken);

            var charges = await _chargeRepository.GetByAssignmentIdAsync(existing.Id, tenantId, branchId, cancellationToken);
            if (charges.Any(x => x.PaidAmount > 0))
                return await RollbackAsync("Cannot delete assignment because some charges are already paid.", cancellationToken);

            var cancelledCharges = new List<StudentCharge>();
            foreach (var charge in charges.Where(x => !x.IsCancelled))
            {
                charge.IsCancelled = true;
                charge.IsSettled = false;
                _chargeRepository.Update(charge);
                cancelledCharges.Add(charge);
            }

            existing.IsActive = false;
            _repo.Update(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (cancelledCharges.Count > 0)
            {
                await _ledgerPostingService.PostChargesAsync(
                    tenantId,
                    branchId,
                    existing.StudentId,
                    existing.StudentAdmissionId,
                    cancelledCharges.Select(ToChargeResponse),
                    cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return (true, "Student fee assignment deactivated successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<(bool CanModify, string Message)> CanModifyAssignmentAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return (false, "Assignment id is required.");

        var existing = await _repo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
        if (existing is null)
            return (false, "Student fee assignment not found.");

        var charges = await _chargeRepository.GetByAssignmentIdAsync(existing.Id, tenantId, branchId, cancellationToken);
        if (charges.Any(x => x.PaidAmount > 0))
            return (false, "Some charges are already paid. Structure change is not allowed.");

        return (true, "Assignment can be modified.");
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

    private async Task<(bool Success, string Message)> RollbackAsync(string message, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        return (false, message);
    }
}