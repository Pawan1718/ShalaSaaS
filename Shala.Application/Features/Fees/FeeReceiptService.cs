//using Shala.Application.Common;
//using Shala.Application.Repositories.Fees;
//using Shala.Domain.Entities.Fees;

//namespace Shala.Application.Features.Fees;

//public class FeeReceiptService : IFeeReceiptService
//{
//    private readonly IFeeReceiptRepository _receiptRepo;
//    private readonly IStudentChargeRepository _chargeRepo;
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly IFeeReceiptNumberGenerator _receiptNoGenerator;

//    public FeeReceiptService(
//        IFeeReceiptRepository receiptRepo,
//        IStudentChargeRepository chargeRepo,
//        IUnitOfWork unitOfWork,
//        IFeeReceiptNumberGenerator receiptNoGenerator)
//    {
//        _receiptRepo = receiptRepo;
//        _chargeRepo = chargeRepo;
//        _unitOfWork = unitOfWork;
//        _receiptNoGenerator = receiptNoGenerator;
//    }

//    public Task<List<FeeReceipt>> GetByStudentIdAsync(
//        int tenantId,
//        int branchId,
//        int studentId,
//        CancellationToken cancellationToken = default)
//    {
//        return _receiptRepo.GetByStudentIdAsync(
//            studentId,
//            tenantId,
//            branchId,
//            cancellationToken);
//    }

//    public Task<FeeReceipt?> GetByIdAsync(
//        int tenantId,
//        int branchId,
//        int id,
//        CancellationToken cancellationToken = default)
//    {
//        return _receiptRepo.GetByIdAsync(
//            id,
//            tenantId,
//            branchId,
//            cancellationToken);
//    }

//    public async Task<(bool Success, string Message, FeeReceipt? Data)> CreateAsync(
//        int tenantId,
//        int branchId,
//        FeeReceipt entity,
//        CancellationToken cancellationToken = default)
//    {
//        if (entity is null)
//            return (false, "Invalid request.", null);

//        if (entity.StudentId <= 0)
//            return (false, "Student is required.", null);

//        if (entity.Allocations == null || entity.Allocations.Count == 0)
//            return (false, "At least one allocation is required.", null);

//        var duplicateChargeIds = entity.Allocations
//            .GroupBy(x => x.StudentChargeId)
//            .Where(x => x.Count() > 1)
//            .Select(x => x.Key)
//            .ToList();

//        if (duplicateChargeIds.Any())
//            return (false, "Duplicate allocations for the same charge are not allowed.", null);

//        if (entity.Allocations.Any(x => x.AllocatedAmount <= 0))
//            return (false, "Allocated amount must be greater than zero.", null);

//        entity.TenantId = tenantId;
//        entity.BranchId = branchId;
//        entity.IsCancelled = false;
//        entity.CancelledOnUtc = null;
//        entity.CancelReason = null;
//        entity.ReceiptNo = await _receiptNoGenerator.GenerateAsync(
//            tenantId,
//            branchId,
//            cancellationToken);

//        await _unitOfWork.BeginTransactionAsync(cancellationToken);

//        try
//        {
//            decimal totalAllocated = 0m;
//            int? admissionId = null;

//            foreach (var allocation in entity.Allocations)
//            {
//                var charge = await _chargeRepo.GetByIdAsync(
//                    allocation.StudentChargeId,
//                    tenantId,
//                    branchId,
//                    cancellationToken);

//                if (charge is null)
//                    return await RollbackWithMessage("One or more charges were not found.", cancellationToken);

//                if (charge.IsCancelled)
//                    return await RollbackWithMessage("Cancelled charge cannot be paid.", cancellationToken);

//                if ((charge.StudentId ?? 0) != entity.StudentId)
//                    return await RollbackWithMessage("All charges must belong to the selected student.", cancellationToken);

//                if (admissionId == null)
//                    admissionId = charge.StudentAdmissionId;
//                else if (admissionId != charge.StudentAdmissionId)
//                    return await RollbackWithMessage("All allocations must belong to the same admission.", cancellationToken);

//                if (allocation.AllocatedAmount > charge.BalanceAmount)
//                    return await RollbackWithMessage("Allocated amount cannot exceed charge balance.", cancellationToken);

//                charge.PaidAmount += allocation.AllocatedAmount;
//                charge.IsSettled = charge.PaidAmount >= charge.NetAmount;

//                _chargeRepo.Update(charge);

//                totalAllocated += allocation.AllocatedAmount;
//            }

//            if (entity.TotalAmount > 0 && entity.TotalAmount != totalAllocated)
//                return await RollbackWithMessage("Receipt total amount must match the sum of allocations.", cancellationToken);

//            entity.TotalAmount = totalAllocated;

//            if (admissionId.HasValue && (!entity.StudentAdmissionId.HasValue || entity.StudentAdmissionId.Value <= 0))
//                entity.StudentAdmissionId = admissionId.Value;

//            await _receiptRepo.AddAsync(entity, cancellationToken);
//            await _unitOfWork.SaveChangesAsync(cancellationToken);
//            await _unitOfWork.CommitTransactionAsync(cancellationToken);

//            return (true, "Fee receipt created successfully.", entity);
//        }
//        catch
//        {
//            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
//            throw;
//        }
//    }

//    public async Task<(bool Success, string Message)> CancelReceiptAsync(
//        int tenantId,
//        int branchId,
//        int id,
//        string? reason = null,
//        CancellationToken cancellationToken = default)
//    {
//        if (id <= 0)
//            return (false, "Receipt id is required.");

//        var receipt = await _receiptRepo.GetByIdAsync(
//            id,
//            tenantId,
//            branchId,
//            cancellationToken);

//        if (receipt is null)
//            return (false, "Fee receipt not found.");

//        if (receipt.IsCancelled)
//            return (false, "Receipt is already cancelled.");

//        if (receipt.Allocations == null || receipt.Allocations.Count == 0)
//            return (false, "Receipt has no allocations to reverse.");

//        await _unitOfWork.BeginTransactionAsync(cancellationToken);

//        try
//        {
//            foreach (var allocation in receipt.Allocations)
//            {
//                var charge = allocation.StudentCharge
//                             ?? await _chargeRepo.GetByIdAsync(
//                                 allocation.StudentChargeId,
//                                 tenantId,
//                                 branchId,
//                                 cancellationToken);

//                if (charge is null)
//                    return await RollbackCancelWithMessage("One or more allocated charges were not found.", cancellationToken);

//                charge.PaidAmount -= allocation.AllocatedAmount;

//                if (charge.PaidAmount < 0)
//                    charge.PaidAmount = 0;

//                charge.IsSettled = charge.PaidAmount >= charge.NetAmount && !charge.IsCancelled;

//                _chargeRepo.Update(charge);
//            }

//            receipt.IsCancelled = true;
//            receipt.CancelledOnUtc = DateTime.UtcNow;
//            receipt.CancelReason = string.IsNullOrWhiteSpace(reason) ? "Cancelled" : reason.Trim();

//            _receiptRepo.Update(receipt);

//            await _unitOfWork.SaveChangesAsync(cancellationToken);
//            await _unitOfWork.CommitTransactionAsync(cancellationToken);

//            return (true, "Fee receipt cancelled successfully.");
//        }
//        catch
//        {
//            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
//            throw;
//        }
//    }

//    private async Task<(bool Success, string Message, FeeReceipt? Data)> RollbackWithMessage(
//        string message,
//        CancellationToken cancellationToken)
//    {
//        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
//        return (false, message, null);
//    }

//    private async Task<(bool Success, string Message)> RollbackCancelWithMessage(
//        string message,
//        CancellationToken cancellationToken)
//    {
//        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
//        return (false, message);
//    }
//}







using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public class FeeReceiptService : IFeeReceiptService
{
    private readonly IFeeReceiptRepository _receiptRepo;
    private readonly IStudentChargeRepository _chargeRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeReceiptNumberGenerator _receiptNoGenerator;
    private readonly IFeeLedgerPostingService _ledgerPostingService;

    public FeeReceiptService(
        IFeeReceiptRepository receiptRepo,
        IStudentChargeRepository chargeRepo,
        IUnitOfWork unitOfWork,
        IFeeReceiptNumberGenerator receiptNoGenerator,
        IFeeLedgerPostingService ledgerPostingService)
    {
        _receiptRepo = receiptRepo;
        _chargeRepo = chargeRepo;
        _unitOfWork = unitOfWork;
        _receiptNoGenerator = receiptNoGenerator;
        _ledgerPostingService = ledgerPostingService;
    }

    public Task<List<FeeReceipt>> GetByStudentIdAsync(
        int tenantId,
        int branchId,
        int studentId,
        CancellationToken cancellationToken = default)
    {
        return _receiptRepo.GetByStudentIdAsync(
            studentId,
            tenantId,
            branchId,
            cancellationToken);
    }

    public Task<FeeReceipt?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        return _receiptRepo.GetByIdAsync(
            id,
            tenantId,
            branchId,
            cancellationToken);
    }

    public async Task<(bool Success, string Message, FeeReceipt? Data)> CreateAsync(
        int tenantId,
        int branchId,
        FeeReceipt entity,
        CancellationToken cancellationToken = default)
    {
        if (entity is null)
            return (false, "Invalid request.", null);

        if (entity.StudentId <= 0)
            return (false, "Student is required.", null);

        if (entity.Allocations == null || entity.Allocations.Count == 0)
            return (false, "At least one allocation is required.", null);

        var duplicateChargeIds = entity.Allocations
            .GroupBy(x => x.StudentChargeId)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicateChargeIds.Any())
            return (false, "Duplicate allocations for the same charge are not allowed.", null);

        if (entity.Allocations.Any(x => x.AllocatedAmount <= 0))
            return (false, "Allocated amount must be greater than zero.", null);

        entity.TenantId = tenantId;
        entity.BranchId = branchId;
        entity.IsCancelled = false;
        entity.CancelledOnUtc = null;
        entity.CancelReason = null;
        entity.ReceiptNo = await _receiptNoGenerator.GenerateAsync(
            tenantId,
            branchId,
            cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            decimal totalAllocated = 0m;
            int? admissionId = null;

            foreach (var allocation in entity.Allocations)
            {
                var charge = await _chargeRepo.GetByIdAsync(
                    allocation.StudentChargeId,
                    tenantId,
                    branchId,
                    cancellationToken);

                if (charge is null)
                    return await RollbackWithMessage("One or more charges were not found.", cancellationToken);

                if (charge.IsCancelled)
                    return await RollbackWithMessage("Cancelled charge cannot be paid.", cancellationToken);

                if ((charge.StudentId ?? 0) != entity.StudentId)
                    return await RollbackWithMessage("All charges must belong to the selected student.", cancellationToken);

                if (admissionId == null)
                    admissionId = charge.StudentAdmissionId;
                else if (admissionId != charge.StudentAdmissionId)
                    return await RollbackWithMessage("All allocations must belong to the same admission.", cancellationToken);

                if (allocation.AllocatedAmount > charge.BalanceAmount)
                    return await RollbackWithMessage("Allocated amount cannot exceed charge balance.", cancellationToken);

                charge.PaidAmount += allocation.AllocatedAmount;
                charge.IsSettled = charge.PaidAmount >= charge.NetAmount;

                _chargeRepo.Update(charge);

                totalAllocated += allocation.AllocatedAmount;
            }

            if (entity.TotalAmount > 0 && entity.TotalAmount != totalAllocated)
                return await RollbackWithMessage("Receipt total amount must match the sum of allocations.", cancellationToken);

            entity.TotalAmount = totalAllocated;

            if (admissionId.HasValue && (!entity.StudentAdmissionId.HasValue || entity.StudentAdmissionId.Value <= 0))
                entity.StudentAdmissionId = admissionId.Value;

            await _receiptRepo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _ledgerPostingService.PostReceiptAsync(
                tenantId,
                branchId,
                new FeeReceiptResponse
                {
                    Id = entity.Id,
                    StudentId = entity.StudentId,
                    StudentAdmissionId = entity.StudentAdmissionId ?? 0,
                    ReceiptDate = entity.ReceiptDate,
                    ReceiptNo = entity.ReceiptNo
                },
                entity.Allocations.Select(x => new FeeReceiptAllocationResponse
                {
                    StudentChargeId = x.StudentChargeId,
                    AllocatedAmount = x.AllocatedAmount
                }),
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return (true, "Fee receipt created successfully.", entity);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<(bool Success, string Message)> CancelReceiptAsync(
        int tenantId,
        int branchId,
        int id,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return (false, "Receipt id is required.");

        var receipt = await _receiptRepo.GetByIdAsync(
            id,
            tenantId,
            branchId,
            cancellationToken);

        if (receipt is null)
            return (false, "Fee receipt not found.");

        if (receipt.IsCancelled)
            return (false, "Receipt is already cancelled.");

        if (receipt.Allocations == null || receipt.Allocations.Count == 0)
            return (false, "Receipt has no allocations to reverse.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var allocation in receipt.Allocations)
            {
                var charge = allocation.StudentCharge
                             ?? await _chargeRepo.GetByIdAsync(
                                 allocation.StudentChargeId,
                                 tenantId,
                                 branchId,
                                 cancellationToken);

                if (charge is null)
                    return await RollbackCancelWithMessage("One or more allocated charges were not found.", cancellationToken);

                charge.PaidAmount -= allocation.AllocatedAmount;

                if (charge.PaidAmount < 0)
                    charge.PaidAmount = 0;

                charge.IsSettled = charge.PaidAmount >= charge.NetAmount && !charge.IsCancelled;

                _chargeRepo.Update(charge);
            }

            receipt.IsCancelled = true;
            receipt.CancelledOnUtc = DateTime.UtcNow;
            receipt.CancelReason = string.IsNullOrWhiteSpace(reason) ? "Cancelled" : reason.Trim();

            _receiptRepo.Update(receipt);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _ledgerPostingService.PostReceiptAsync(
                tenantId,
                branchId,
                new FeeReceiptResponse
                {
                    Id = receipt.Id,
                    StudentId = receipt.StudentId,
                    StudentAdmissionId = receipt.StudentAdmissionId ?? 0,
                    ReceiptDate = DateTime.UtcNow,
                    ReceiptNo = receipt.ReceiptNo
                },
                receipt.Allocations.Select(x => new FeeReceiptAllocationResponse
                {
                    StudentChargeId = x.StudentChargeId,
                    AllocatedAmount = -x.AllocatedAmount
                }),
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return (true, "Fee receipt cancelled successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<(bool Success, string Message, FeeReceipt? Data)> RollbackWithMessage(
        string message,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        return (false, message, null);
    }

    private async Task<(bool Success, string Message)> RollbackCancelWithMessage(
        string message,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        return (false, message);
    }
}