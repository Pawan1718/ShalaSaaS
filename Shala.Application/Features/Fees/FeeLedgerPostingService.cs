using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Application.Features.Fees;

public sealed class FeeLedgerPostingService : IFeeLedgerPostingService
{
    private readonly IFeeLedgerWriteRepository _ledgerRepository;

    public FeeLedgerPostingService(IFeeLedgerWriteRepository ledgerRepository)
    {
        _ledgerRepository = ledgerRepository;
    }

    public async Task PostChargesAsync(
     int tenantId,
     int branchId,
     int studentId,
     int studentAdmissionId,
     IEnumerable<StudentChargeResponse> charges,
     CancellationToken cancellationToken = default)
    {
        var validCharges = charges
            .Where(x => !x.IsCancelled)
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Id)
            .ToList();

        if (!validCharges.Any())
            return;

        var chargeIds = validCharges
            .Where(x => x.Id > 0)
            .Select(x => x.Id)
            .Distinct()
            .ToList();

        if (chargeIds.Count > 0)
        {
            await _ledgerRepository.DeleteByChargeIdsAsync(
                tenantId,
                branchId,
                chargeIds,
                cancellationToken);
        }

        var entries = new List<StudentFeeLedger>();

        foreach (var charge in validCharges)
        {
            var debit = charge.Amount + charge.FineAmount - charge.DiscountAmount;
            if (debit <= 0)
                continue;

            entries.Add(new StudentFeeLedger
            {
                TenantId = tenantId,
                BranchId = branchId,
                StudentId = studentId,
                StudentAdmissionId = studentAdmissionId,
                StudentChargeId = charge.Id,
                FeeHeadId = charge.FeeHeadId,
                EntryType = "Charge",
                EntryDate = charge.DueDate == default ? DateTime.UtcNow : charge.DueDate,
                DebitAmount = debit,
                CreditAmount = 0m,
                RunningBalance = 0m,
                ReferenceNo = $"CH-{charge.Id}",
                Remarks = "Charge posted"
            });
        }

        if (entries.Count == 0)
            return;

        await _ledgerRepository.AddRangeAsync(entries, cancellationToken);
        await _ledgerRepository.SaveChangesAsync(cancellationToken);

        await _ledgerRepository.RebuildRunningBalanceAsync(
            tenantId,
            branchId,
            studentAdmissionId,
            cancellationToken);

        await _ledgerRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task PostReceiptAsync(
     int tenantId,
     int branchId,
     FeeReceiptResponse receipt,
     IEnumerable<FeeReceiptAllocationResponse> allocations,
     CancellationToken cancellationToken = default)
    {
        var validAllocations = allocations
            .Where(x => x.AllocatedAmount != 0)
            .OrderBy(x => x.StudentChargeId)
            .ToList();

        if (!validAllocations.Any())
            return;

        // ❗ IMPORTANT: remove old ledger entries of this receipt (idempotent safe)
        var existingEntries = await _ledgerRepository.GetByReceiptIdAsync(
            tenantId,
            branchId,
            receipt.Id,
            cancellationToken);

        if (existingEntries.Any())
        {
            _ledgerRepository.RemoveRange(existingEntries);
            await _ledgerRepository.SaveChangesAsync(cancellationToken);
        }

        var entries = new List<StudentFeeLedger>();

        foreach (var allocation in validAllocations)
        {
            var amount = Math.Abs(allocation.AllocatedAmount);
            if (amount <= 0)
                continue;

            var isCancellation = receipt.IsCancelled; // 🔥 REAL FIX

            entries.Add(new StudentFeeLedger
            {
                TenantId = tenantId,
                BranchId = branchId,
                StudentId = receipt.StudentId,
                StudentAdmissionId = receipt.StudentAdmissionId,
                StudentChargeId = allocation.StudentChargeId,
                FeeReceiptId = receipt.Id,
                FeeHeadId = allocation.FeeHeadId > 0 ? allocation.FeeHeadId : null,
                EntryType = isCancellation ? "ReceiptCancel" : "Receipt",
                EntryDate = receipt.ReceiptDate == default ? DateTime.UtcNow : receipt.ReceiptDate,

                // 🔥 CORE ACCOUNTING FIX
                DebitAmount = isCancellation ? amount : 0m,
                CreditAmount = isCancellation ? 0m : amount,

                RunningBalance = 0m,
                ReferenceNo = receipt.ReceiptNo,
                Remarks = isCancellation
                    ? "Receipt cancelled (reversal entry)"
                    : "Receipt posted"
            });
        }

        if (entries.Count == 0)
            return;

        await _ledgerRepository.AddRangeAsync(entries, cancellationToken);
        await _ledgerRepository.SaveChangesAsync(cancellationToken);

        // 🔥 ALWAYS rebuild after financial mutation
        await _ledgerRepository.RebuildRunningBalanceAsync(
            tenantId,
            branchId,
            receipt.StudentAdmissionId,
            cancellationToken);

        await _ledgerRepository.SaveChangesAsync(cancellationToken);
    }
}