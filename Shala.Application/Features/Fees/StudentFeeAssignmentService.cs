using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;

namespace Shala.Application.Features.Fees;

public class StudentFeeAssignmentService : IStudentFeeAssignmentService
{
    private readonly IStudentFeeAssignmentRepository _repo;
    private readonly IStudentChargeRepository _chargeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentFeeAssignmentService(
        IStudentFeeAssignmentRepository repo,
        IStudentChargeRepository chargeRepository,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _chargeRepository = chargeRepository;
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

        var existing = await _repo.GetByAdmissionIdAsync(
            entity.StudentAdmissionId,
            tenantId,
            branchId,
            cancellationToken);

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

        var existing = await _repo.GetByIdAsync(entity.Id, tenantId, branchId, cancellationToken);

        if (existing is null)
            return (false, "Student fee assignment not found.");

        var existingCharges = await _chargeRepository.GetByAssignmentIdAsync(
            existing.Id,
            tenantId,
            branchId,
            cancellationToken);

        var hasPaidCharges = existingCharges.Any(x => x.PaidAmount > 0);
        var structureChanged = existing.FeeStructureId != entity.FeeStructureId;

        if (hasPaidCharges && structureChanged)
            return (false, "Cannot change fee structure because some charges are already paid.");

        existing.FeeStructureId = entity.FeeStructureId;
        existing.DiscountAmount = entity.DiscountAmount;
        existing.AdditionalChargeAmount = entity.AdditionalChargeAmount;
        existing.IsActive = entity.IsActive;

        _repo.Update(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Student fee assignment updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(
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

        var charges = await _chargeRepository.GetByAssignmentIdAsync(
            existing.Id,
            tenantId,
            branchId,
            cancellationToken);

        if (charges.Any(x => x.PaidAmount > 0))
            return (false, "Cannot delete assignment because some charges are already paid.");

        if (charges.Any())
            await _chargeRepository.DeleteRangeAsync(charges, cancellationToken);

        _repo.Delete(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Student fee assignment deleted successfully.");
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

        var charges = await _chargeRepository.GetByAssignmentIdAsync(
            existing.Id,
            tenantId,
            branchId,
            cancellationToken);

        if (charges.Any(x => x.PaidAmount > 0))
            return (false, "Some charges are already paid. Structure change is not allowed.");

        return (true, "Assignment can be modified.");
    }
}