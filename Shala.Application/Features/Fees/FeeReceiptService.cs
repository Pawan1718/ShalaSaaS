using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Common;
using Shala.Shared.Responses.Fees;
using System.Data;

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
        return _receiptRepo.GetByStudentIdAsync(studentId, tenantId, branchId, cancellationToken);
    }

    public Task<FeeReceipt?> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        return _receiptRepo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
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

        var sanitizedAllocations = entity.Allocations
            .Where(x => x.StudentChargeId > 0 && x.AllocatedAmount > 0)
            .Select(x => new FeeReceiptAllocation
            {
                StudentChargeId = x.StudentChargeId,
                AllocatedAmount = decimal.Round(x.AllocatedAmount, 2, MidpointRounding.AwayFromZero)
            })
            .ToList();

        if (sanitizedAllocations.Count == 0)
            return (false, "At least one valid allocation is required.", null);

        var duplicateChargeIds = sanitizedAllocations
            .GroupBy(x => x.StudentChargeId)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicateChargeIds.Count > 0)
            return (false, "Duplicate allocations for the same charge are not allowed.", null);

        entity.TenantId = tenantId;
        entity.BranchId = branchId;
        entity.IsCancelled = false;
        entity.CancelledOnUtc = null;
        entity.CancelReason = null;
        entity.Allocations = sanitizedAllocations;

        await _unitOfWork.BeginTransactionAsync(cancellationToken, IsolationLevel.Serializable);

        try
        {
            entity.ReceiptNo = await _receiptNoGenerator.GenerateAsync(tenantId, branchId, cancellationToken);

            decimal totalAllocated = 0m;
            int? admissionId = null;
            var allocationResponses = new List<FeeReceiptAllocationResponse>();

            foreach (var allocation in entity.Allocations.OrderBy(x => x.StudentChargeId))
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

                if (!charge.StudentAdmissionId.HasValue || charge.StudentAdmissionId.Value <= 0)
                    return await RollbackWithMessage("Charge must belong to a valid admission.", cancellationToken);

                if (admissionId is null)
                    admissionId = charge.StudentAdmissionId.Value;
                else if (admissionId.Value != charge.StudentAdmissionId.Value)
                    return await RollbackWithMessage("All allocations must belong to the same admission.", cancellationToken);

                if (entity.StudentAdmissionId.HasValue && entity.StudentAdmissionId.Value > 0 && entity.StudentAdmissionId.Value != charge.StudentAdmissionId.Value)
                    return await RollbackWithMessage("Receipt admission does not match allocated charges.", cancellationToken);

                var openBalance = decimal.Round(charge.BalanceAmount, 2, MidpointRounding.AwayFromZero);
                var allocatedAmount = decimal.Round(allocation.AllocatedAmount, 2, MidpointRounding.AwayFromZero);

                if (allocatedAmount <= 0)
                    return await RollbackWithMessage("Allocated amount must be greater than zero.", cancellationToken);

                if (allocatedAmount > openBalance)
                    return await RollbackWithMessage("Allocated amount cannot exceed charge balance.", cancellationToken);

                charge.PaidAmount = decimal.Round(charge.PaidAmount + allocatedAmount, 2, MidpointRounding.AwayFromZero);
                charge.IsSettled = charge.PaidAmount >= charge.NetAmount && !charge.IsCancelled;
                _chargeRepo.Update(charge);

                totalAllocated += allocatedAmount;
                allocation.AllocatedAmount = allocatedAmount;

                allocationResponses.Add(new FeeReceiptAllocationResponse
                {
                    StudentChargeId = charge.Id,
                    FeeHeadId = charge.FeeHeadId,
                    ChargeLabel = charge.ChargeLabel,
                    FeeHeadName = charge.FeeHead?.Name,
                    PeriodLabel = charge.PeriodLabel,
                    DueDate = charge.DueDate,
                    AllocatedAmount = allocatedAmount
                });
            }

            totalAllocated = decimal.Round(totalAllocated, 2, MidpointRounding.AwayFromZero);

            if (entity.TotalAmount > 0 && decimal.Round(entity.TotalAmount, 2, MidpointRounding.AwayFromZero) != totalAllocated)
                return await RollbackWithMessage("Receipt total amount must match the sum of allocations.", cancellationToken);

            entity.TotalAmount = totalAllocated;
            entity.StudentAdmissionId = admissionId;
            entity.ReceiptDate = entity.ReceiptDate == default ? DateTime.UtcNow : entity.ReceiptDate;

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
                    ReceiptNo = entity.ReceiptNo,
                    TotalAmount = entity.TotalAmount,
                    IsCancelled = false
                },
                allocationResponses,
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

        await _unitOfWork.BeginTransactionAsync(cancellationToken, IsolationLevel.Serializable);

        try
        {
            var receipt = await _receiptRepo.GetByIdAsync(id, tenantId, branchId, cancellationToken);
            if (receipt is null)
                return await RollbackCancelWithMessage("Fee receipt not found.", cancellationToken);

            if (receipt.IsCancelled)
                return await RollbackCancelWithMessage("Receipt is already cancelled.", cancellationToken);

            if (receipt.Allocations == null || receipt.Allocations.Count == 0)
                return await RollbackCancelWithMessage("Receipt has no allocations to reverse.", cancellationToken);

            var allocationResponses = new List<FeeReceiptAllocationResponse>();

            foreach (var allocation in receipt.Allocations.OrderBy(x => x.StudentChargeId))
            {
                var charge = allocation.StudentCharge ?? await _chargeRepo.GetByIdAsync(
                    allocation.StudentChargeId,
                    tenantId,
                    branchId,
                    cancellationToken);

                if (charge is null)
                    return await RollbackCancelWithMessage("One or more allocated charges were not found.", cancellationToken);

                if (charge.PaidAmount < allocation.AllocatedAmount)
                    return await RollbackCancelWithMessage("Receipt cancellation would make charge paid amount negative.", cancellationToken);

                charge.PaidAmount = decimal.Round(charge.PaidAmount - allocation.AllocatedAmount, 2, MidpointRounding.AwayFromZero);
                if (charge.PaidAmount < 0)
                    charge.PaidAmount = 0;

                charge.IsSettled = charge.PaidAmount >= charge.NetAmount && !charge.IsCancelled;
                _chargeRepo.Update(charge);

                allocationResponses.Add(new FeeReceiptAllocationResponse
                {
                    StudentChargeId = charge.Id,
                    FeeHeadId = charge.FeeHeadId,
                    ChargeLabel = charge.ChargeLabel,
                    FeeHeadName = charge.FeeHead?.Name,
                    PeriodLabel = charge.PeriodLabel,
                    DueDate = charge.DueDate,
                    AllocatedAmount = allocation.AllocatedAmount
                });
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
                    ReceiptDate = receipt.ReceiptDate,
                    ReceiptNo = receipt.ReceiptNo,
                    TotalAmount = receipt.TotalAmount,
                    IsCancelled = true,
                    CancelledOnUtc = receipt.CancelledOnUtc,
                    CancelReason = receipt.CancelReason
                },
                allocationResponses,
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

    public async Task<PagedResult<FeeReceiptResponse>> GetPagedByStudentIdAsync(
     int tenantId,
     int branchId,
     int studentId,
     int pageNumber,
     int pageSize,
     CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var (items, totalCount) = await _receiptRepo.GetPagedByStudentIdAsync(
            studentId,
            tenantId,
            branchId,
            pageNumber,
            pageSize,
            cancellationToken);

        return new PagedResult<FeeReceiptResponse>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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