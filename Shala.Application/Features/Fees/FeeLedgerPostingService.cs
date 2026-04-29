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
        var chargeList = charges
            .Where(x => x.Id > 0)
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Id)
            .ToList();

        if (chargeList.Count == 0)
            return;

        var existingEntries = await _ledgerRepository.GetByChargeIdsAsync(
            tenantId,
            branchId,
            chargeList.Select(x => x.Id),
            cancellationToken);

        var existingChargeEntryByChargeId = existingEntries
            .Where(x => x.StudentChargeId.HasValue && x.EntryType == "Charge")
            .GroupBy(x => x.StudentChargeId!.Value)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Id).First());

        var existingChargeCancelEntryIds = existingEntries
            .Where(x => x.StudentChargeId.HasValue && x.EntryType == "ChargeCancel")
            .Select(x => x.StudentChargeId!.Value)
            .ToHashSet();

        var newEntries = new List<StudentFeeLedger>();

        foreach (var charge in chargeList)
        {
            var netAmount = charge.Amount + charge.FineAmount - charge.DiscountAmount;
            if (netAmount <= 0)
                continue;

            var entryDate = charge.DueDate == default ? DateTime.UtcNow : charge.DueDate;
            var hasChargeEntry = existingChargeEntryByChargeId.ContainsKey(charge.Id);
            var hasChargeCancelEntry = existingChargeCancelEntryIds.Contains(charge.Id);

            if (!charge.IsCancelled)
            {
                if (hasChargeEntry)
                    continue;

                newEntries.Add(new StudentFeeLedger
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    StudentId = studentId,
                    StudentAdmissionId = studentAdmissionId,
                    StudentChargeId = charge.Id,
                    FeeHeadId = charge.FeeHeadId,
                    EntryType = "Charge",
                    EntryDate = entryDate,
                    DebitAmount = netAmount,
                    CreditAmount = 0m,
                    RunningBalance = 0m,
                    ReferenceNo = $"CH-{charge.Id}",
                    Remarks = "Charge posted"
                });

                continue;
            }

            if (!hasChargeEntry || hasChargeCancelEntry)
                continue;

            newEntries.Add(new StudentFeeLedger
            {
                TenantId = tenantId,
                BranchId = branchId,
                StudentId = studentId,
                StudentAdmissionId = studentAdmissionId,
                StudentChargeId = charge.Id,
                FeeHeadId = charge.FeeHeadId,
                EntryType = "ChargeCancel",
                EntryDate = DateTime.UtcNow,
                DebitAmount = 0m,
                CreditAmount = netAmount,
                RunningBalance = 0m,
                ReferenceNo = $"CH-CN-{charge.Id}",
                Remarks = "Charge cancelled"
            });
        }

        if (newEntries.Count == 0)
            return;

        await _ledgerRepository.AddRangeAsync(newEntries, cancellationToken);
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
        var allocationList = allocations
            .Where(x => x.StudentChargeId > 0 && Math.Abs(x.AllocatedAmount) > 0)
            .OrderBy(x => x.StudentChargeId)
            .ToList();

        if (allocationList.Count == 0)
            return;

        var existingEntries = await _ledgerRepository.GetByReceiptIdAsync(
            tenantId,
            branchId,
            receipt.Id,
            cancellationToken);

        var existingReceiptChargeIds = existingEntries
            .Where(x => x.EntryType == "Receipt" && x.StudentChargeId.HasValue)
            .Select(x => x.StudentChargeId!.Value)
            .ToHashSet();

        var existingReceiptCancelChargeIds = existingEntries
            .Where(x => x.EntryType == "ReceiptCancel" && x.StudentChargeId.HasValue)
            .Select(x => x.StudentChargeId!.Value)
            .ToHashSet();

        var isCancellation = receipt.IsCancelled;
        var entryDate = isCancellation
            ? (receipt.CancelledOnUtc ?? DateTime.UtcNow)
            : (receipt.ReceiptDate == default ? DateTime.UtcNow : receipt.ReceiptDate);

        var newEntries = new List<StudentFeeLedger>();

        foreach (var allocation in allocationList)
        {
            var amount = Math.Abs(allocation.AllocatedAmount);
            if (amount <= 0)
                continue;

            if (!isCancellation)
            {
                if (existingReceiptChargeIds.Contains(allocation.StudentChargeId))
                    continue;

                newEntries.Add(new StudentFeeLedger
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    StudentId = receipt.StudentId,
                    StudentAdmissionId = receipt.StudentAdmissionId,
                    StudentChargeId = allocation.StudentChargeId,
                    FeeReceiptId = receipt.Id,
                    FeeHeadId = allocation.FeeHeadId > 0 ? allocation.FeeHeadId : null,
                    EntryType = "Receipt",
                    EntryDate = entryDate,
                    DebitAmount = 0m,
                    CreditAmount = amount,
                    RunningBalance = 0m,
                    ReferenceNo = receipt.ReceiptNo,
                    Remarks = "Receipt posted"
                });

                continue;
            }

            if (existingReceiptCancelChargeIds.Contains(allocation.StudentChargeId))
                continue;

            newEntries.Add(new StudentFeeLedger
            {
                TenantId = tenantId,
                BranchId = branchId,
                StudentId = receipt.StudentId,
                StudentAdmissionId = receipt.StudentAdmissionId,
                StudentChargeId = allocation.StudentChargeId,
                FeeReceiptId = receipt.Id,
                FeeHeadId = allocation.FeeHeadId > 0 ? allocation.FeeHeadId : null,
                EntryType = "ReceiptCancel",
                EntryDate = entryDate,
                DebitAmount = amount,
                CreditAmount = 0m,
                RunningBalance = 0m,
                ReferenceNo = receipt.ReceiptNo,
                Remarks = "Receipt cancelled"
            });
        }

        if (newEntries.Count == 0)
            return;

        await _ledgerRepository.AddRangeAsync(newEntries, cancellationToken);
        await _ledgerRepository.SaveChangesAsync(cancellationToken);

        await _ledgerRepository.RebuildRunningBalanceAsync(
            tenantId,
            branchId,
            receipt.StudentAdmissionId,
            cancellationToken);

        await _ledgerRepository.SaveChangesAsync(cancellationToken);
    }
}