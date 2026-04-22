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

        var runningBalance = await _ledgerRepository.GetLatestRunningBalanceAsync(
            tenantId,
            branchId,
            studentAdmissionId,
            cancellationToken);

        var entries = new List<StudentFeeLedger>();

        foreach (var charge in validCharges)
        {
            var debit = charge.Amount + charge.FineAmount - charge.DiscountAmount;
            if (debit <= 0)
                continue;

            runningBalance += debit;

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
                RunningBalance = runningBalance,
                ReferenceNo = $"CH-{charge.Id}",
                Remarks = $"Charge posted"
            });
        }

        if (!entries.Any())
            return;

        await _ledgerRepository.AddRangeAsync(entries, cancellationToken);
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
            .Where(x => x.AllocatedAmount > 0)
            .OrderBy(x => x.StudentChargeId)
            .ToList();

        if (!validAllocations.Any())
            return;

        var runningBalance = await _ledgerRepository.GetLatestRunningBalanceAsync(
            tenantId,
            branchId,
            receipt.StudentAdmissionId,
            cancellationToken);

        var entries = new List<StudentFeeLedger>();

        foreach (var allocation in validAllocations)
        {
            runningBalance -= allocation.AllocatedAmount;

            entries.Add(new StudentFeeLedger
            {
                TenantId = tenantId,
                BranchId = branchId,
                StudentId = receipt.StudentId,
                StudentAdmissionId = receipt.StudentAdmissionId,
                StudentChargeId = allocation.StudentChargeId,
                FeeReceiptId = receipt.Id,
                EntryType = "Receipt",
                EntryDate = receipt.ReceiptDate,
                DebitAmount = 0m,
                CreditAmount = allocation.AllocatedAmount,
                RunningBalance = runningBalance,
                ReferenceNo = receipt.ReceiptNo,
                Remarks = "Fee receipt posted"
            });
        }

        await _ledgerRepository.AddRangeAsync(entries, cancellationToken);
        await _ledgerRepository.SaveChangesAsync(cancellationToken);
    }
}