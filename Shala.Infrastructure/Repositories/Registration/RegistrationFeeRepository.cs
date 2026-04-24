using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Registration;
using Shala.Domain.Entities.Registration;
using Shala.Domain.Enum;
using Shala.Domain.Enums;
using Shala.Infrastructure.Data;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Infrastructure.Repositories.Registration
{
    public class RegistrationFeeRepository : IRegistrationFeeRepository
    {
        private readonly AppDbContext _db;

        public RegistrationFeeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RegistrationFeeResponse> SaveWithFeeAsync(
            int tenantId,
            int branchId,
            SaveRegistrationWithFeeRequest request,
            CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.Registration is null)
                throw new ArgumentException("Registration details are required.", nameof(request));

            if (request.Fee is null)
                throw new ArgumentException("Fee details are required.", nameof(request));

            var feeConfig = await GetFeeConfigAsync(tenantId, branchId, ct);
            var prospectusConfig = await GetProspectusConfigAsync(tenantId, branchId, ct);
            var receiptConfig = await GetReceiptConfigAsync(tenantId, branchId, ct);

            var feeContext = BuildFeeContext(feeConfig, prospectusConfig, request.Fee);

            await using var transaction = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                var registration = new StudentRegistration
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    RegistrationDate = request.Registration.RegistrationDate,
                    FirstName = request.Registration.FirstName.Trim(),
                    MiddleName = NormalizeText(request.Registration.MiddleName),
                    LastName = request.Registration.LastName.Trim(),
                    GuardianName = request.Registration.GuardianName.Trim(),
                    Phone = request.Registration.Phone.Trim(),
                    Address = NormalizeText(request.Registration.Address),
                    Note = NormalizeText(request.Registration.Note),
                    InterestedClassId = request.Registration.InterestedClassId,
                    DateOfBirth = request.Registration.DateOfBirth,
                    Gender = request.Registration.Gender,
                    FeePaid = feeContext.TotalAmount > 0m,
                    PaymentStatus = feeContext.TotalAmount > 0m
    ? RegistrationPaymentStatus.Paid
    : RegistrationPaymentStatus.Unpaid,
                    Status = feeContext.TotalAmount > 0m
    ? RegistrationStatus.Confirmed
    : RegistrationStatus.Pending,
                    IsDeleted = false,
                    RegistrationNo = string.Empty
                };

                await _db.StudentRegistrations.AddAsync(registration, ct);
                await _db.SaveChangesAsync(ct);

                registration.RegistrationNo = $"REG-{DateTime.UtcNow:yyyy}-{registration.Id:D5}";
                await _db.SaveChangesAsync(ct);

                RegistrationFeeReceipt? receipt = null;

                if (feeContext.TotalAmount > 0m)
                {
                    receipt = await CreateReceiptAsync(
                        tenantId,
                        branchId,
                        registration.Id,
                        request.Fee,
                        feeContext,
                        ct);
                }

                await transaction.CommitAsync(ct);

                return BuildFeeResponse(registration, receipt, receiptConfig, feeContext);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<RegistrationFeeResponse> CollectAsync(
            int tenantId,
            int branchId,
            int registrationId,
            CollectRegistrationFeeRequest request,
            CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);

            var feeConfig = await GetFeeConfigAsync(tenantId, branchId, ct);
            var prospectusConfig = await GetProspectusConfigAsync(tenantId, branchId, ct);
            var receiptConfig = await GetReceiptConfigAsync(tenantId, branchId, ct);

            var feeContext = BuildFeeContext(feeConfig, prospectusConfig, request);

            if (feeContext.TotalAmount <= 0m)
                throw new InvalidOperationException("Total payable amount must be greater than zero.");

            await using var transaction = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                var registration = await _db.StudentRegistrations
                    .FirstOrDefaultAsync(
                        x => x.Id == registrationId &&
                             x.TenantId == tenantId &&
                             x.BranchId == branchId &&
                             !x.IsDeleted,
                        ct);

                if (registration is null)
                    throw new KeyNotFoundException("Registration not found.");

                if (registration.PaymentStatus == RegistrationPaymentStatus.Paid || registration.FeePaid)
                    throw new InvalidOperationException("Fee already collected.");

                registration.FeePaid = true;
                registration.PaymentStatus = RegistrationPaymentStatus.Paid;
                registration.Status = RegistrationStatus.Confirmed;

                var receipt = await CreateReceiptAsync(
                    tenantId,
                    branchId,
                    registration.Id,
                    request,
                    feeContext,
                    ct);

                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return BuildFeeResponse(registration, receipt, receiptConfig, feeContext);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<RegistrationReceiptResponse> GetReceiptAsync(
            int tenantId,
            int branchId,
            int receiptId,
            CancellationToken ct)
        {
            var receiptConfig = await GetReceiptConfigAsync(tenantId, branchId, ct);
            var prospectusConfig = await GetProspectusConfigAsync(tenantId, branchId, ct);

            var data = await (
                from receipt in _db.RegistrationFeeReceipts.AsNoTracking()
                join registration in _db.StudentRegistrations.AsNoTracking()
                    on receipt.RegistrationId equals registration.Id
                where receipt.Id == receiptId
       && receipt.TenantId == tenantId
       && receipt.BranchId == branchId
       && !receipt.IsCancelled
                select new
                {
                    Receipt = receipt,
                    Registration = registration
                }).FirstOrDefaultAsync(ct);

            if (data is null)
                throw new KeyNotFoundException("Receipt not found.");

            var lines = new List<RegistrationReceiptLineResponse>();

            if (data.Receipt.RegistrationAmount > 0m)
            {
                lines.Add(new RegistrationReceiptLineResponse
                {
                    Label = "Registration Fee",
                    Amount = data.Receipt.RegistrationAmount
                });
            }

            if (data.Receipt.IsProspectusIncluded && data.Receipt.ProspectusAmount > 0m)
            {
                lines.Add(new RegistrationReceiptLineResponse
                {
                    Label = !string.IsNullOrWhiteSpace(data.Receipt.ProspectusLabel)
                        ? data.Receipt.ProspectusLabel!
                        : !string.IsNullOrWhiteSpace(prospectusConfig?.ProspectusDisplayName)
                            ? prospectusConfig.ProspectusDisplayName!
                            : "Prospectus",
                    Amount = data.Receipt.ProspectusAmount
                });
            }

            return new RegistrationReceiptResponse
            {
                ReceiptId = data.Receipt.Id,
                ReceiptNo = data.Receipt.ReceiptNo,
                ReceiptDate = data.Receipt.ReceiptDate,
                RegistrationId = data.Registration.Id,
                RegistrationNo = data.Registration.RegistrationNo,
                StudentName = string.Join(" ", new[]
                {
                    data.Registration.FirstName,
                    data.Registration.MiddleName,
                    data.Registration.LastName
                }.Where(x => !string.IsNullOrWhiteSpace(x))),
                GuardianName = data.Registration.GuardianName,
                Phone = data.Registration.Phone,
                RegistrationDate = data.Registration.RegistrationDate,
                ReceiptTitle = NormalizeText(receiptConfig?.ReceiptTitle) ?? "Registration Receipt",
                ReceiptFooterNote = NormalizeText(receiptConfig?.ReceiptFooterNote) ?? NormalizeText(data.Receipt.Remarks),
                ShowStudentDetailsInReceipt = receiptConfig?.ShowStudentDetailsInReceipt ?? true,
                ShowFeeHeadInReceipt = receiptConfig?.ShowFeeHeadInReceipt ?? true,
                ShowAmountInWords = receiptConfig?.ShowAmountInWords ?? true,
                ShowProspectusInReceipt = prospectusConfig?.ShowProspectusInReceipt ?? true,
                CanPrintReceipt = receiptConfig?.AllowPrintReceipt ?? true,
                CanDownloadReceipt = receiptConfig?.AllowDownloadReceipt ?? true,
                AutoPrintAfterSave = receiptConfig?.AutoPrintAfterSave ?? false,
                TotalAmount = data.Receipt.TotalAmount,
                Lines = lines
            };
        }

        private async Task<RegistrationFeeReceipt> CreateReceiptAsync(
            int tenantId,
            int branchId,
            int registrationId,
            CollectRegistrationFeeRequest request,
            FeeComputation feeContext,
            CancellationToken ct)
        {
            var receipt = new RegistrationFeeReceipt
            {
                TenantId = tenantId,
                BranchId = branchId,
                RegistrationId = registrationId,
                ReceiptNo = await GenerateReceiptNoAsync(tenantId, branchId, ct),
                ReceiptDate = DateTime.UtcNow,
                PaymentMode = request.PaymentMode,
                TransactionReference = NormalizeText(request.TransactionReference),
                Remarks = NormalizeText(request.Remarks),
                RegistrationAmount = feeContext.RegistrationAmount,
                IsProspectusIncluded = feeContext.IncludeProspectus,
                ProspectusAmount = feeContext.ProspectusAmount,
                ProspectusLabel = feeContext.IncludeProspectus ? feeContext.ProspectusLabel : null,
                TotalAmount = feeContext.TotalAmount,
                IsRegistrationReceipt = true
            };

            await _db.RegistrationFeeReceipts.AddAsync(receipt, ct);
            await _db.SaveChangesAsync(ct);

            return receipt;
        }

        private static RegistrationFeeResponse BuildFeeResponse(
            StudentRegistration registration,
            RegistrationFeeReceipt? receipt,
            RegistrationReceiptConfiguration? receiptConfig,
            FeeComputation feeContext)
        {
            return new RegistrationFeeResponse
            {
                RegistrationId = registration.Id,
                RegistrationNo = registration.RegistrationNo,
                ChargeId = 0,
                ProspectusChargeId = null,
                ReceiptId = receipt?.Id ?? 0,
                ReceiptNo = receipt?.ReceiptNo ?? string.Empty,
                RegistrationDate = registration.RegistrationDate,
                ReceiptDate = receipt?.ReceiptDate ?? default,
                RegistrationAmount = feeContext.RegistrationAmount,
                ProspectusAmount = feeContext.ProspectusAmount,
                TotalAmount = feeContext.TotalAmount,
                CanPrintReceipt = receiptConfig?.AllowPrintReceipt ?? true,
                CanDownloadReceipt = receiptConfig?.AllowDownloadReceipt ?? true,
                AutoPrintAfterSave = receiptConfig?.AutoPrintAfterSave ?? false
            };
        }

        private static FeeComputation BuildFeeContext(
            RegistrationFeeConfiguration? feeConfig,
            RegistrationProspectusConfiguration? prospectusConfig,
            CollectRegistrationFeeRequest request)
        {
            var registrationAmount = NormalizeMoney(request.Amount);

            var prospectusEnabled = prospectusConfig?.IsActive == true && prospectusConfig.IncludeProspectus;
            var prospectusMandatory = prospectusEnabled && prospectusConfig!.IsProspectusMandatory;

            var includeProspectus = prospectusEnabled &&
                                    (prospectusMandatory || request.IncludeProspectus);

            decimal prospectusAmount = 0m;

            if (includeProspectus)
            {
                var requestedAmount = NormalizeMoney(request.ProspectusAmount);
                var configuredAmount = prospectusConfig?.ProspectusAmount ?? 0m;

                prospectusAmount = requestedAmount > 0m
                    ? requestedAmount
                    : configuredAmount;
            }

            ValidateFeeRequest(
                feeConfig,
                prospectusConfig,
                request,
                registrationAmount,
                prospectusAmount,
                includeProspectus);

            return new FeeComputation
            {
                RegistrationAmount = registrationAmount,
                IncludeProspectus = includeProspectus,
                ProspectusAmount = prospectusAmount,
                ProspectusLabel = includeProspectus
                    ? NormalizeText(prospectusConfig?.ProspectusDisplayName) ?? "Prospectus"
                    : null,
                TotalAmount = registrationAmount + prospectusAmount
            };
        }

        private async Task<RegistrationFeeConfiguration?> GetFeeConfigAsync(
            int tenantId,
            int branchId,
            CancellationToken ct)
        {
            return await _db.RegistrationFeeConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId &&
                         x.BranchId == branchId &&
                         x.IsActive,
                    ct);
        }

        private async Task<RegistrationProspectusConfiguration?> GetProspectusConfigAsync(
            int tenantId,
            int branchId,
            CancellationToken ct)
        {
            return await _db.RegistrationProspectusConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId &&
                         x.BranchId == branchId &&
                         x.IsActive,
                    ct);
        }

        private async Task<RegistrationReceiptConfiguration?> GetReceiptConfigAsync(
            int tenantId,
            int branchId,
            CancellationToken ct)
        {
            return await _db.RegistrationReceiptConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId &&
                         x.BranchId == branchId &&
                         x.IsActive,
                    ct);
        }

        private static void ValidateFeeRequest(
            RegistrationFeeConfiguration? feeConfig,
            RegistrationProspectusConfiguration? prospectusConfig,
            CollectRegistrationFeeRequest request,
            decimal registrationAmount,
            decimal prospectusAmount,
            bool includeProspectus)
        {
            if (registrationAmount < 0m)
                throw new InvalidOperationException("Registration fee amount cannot be negative.");

            if (feeConfig?.IsRegistrationFeeMandatory == true && registrationAmount <= 0m)
                throw new InvalidOperationException("Registration fee amount must be greater than zero.");

            if (request.PaymentMode != PaymentMode.Cash &&
                string.IsNullOrWhiteSpace(request.TransactionReference))
            {
                throw new InvalidOperationException("Transaction reference is required for non-cash payment.");
            }

            var prospectusEnabled = prospectusConfig?.IsActive == true && prospectusConfig.IncludeProspectus;
            var prospectusMandatory = prospectusEnabled && prospectusConfig?.IsProspectusMandatory == true;

            if (!prospectusEnabled && request.IncludeProspectus)
                throw new InvalidOperationException("Prospectus is not enabled for this branch.");

            if (prospectusMandatory && !includeProspectus)
                throw new InvalidOperationException("Prospectus is mandatory.");

            if (includeProspectus && prospectusAmount < 0m)
                throw new InvalidOperationException("Prospectus amount cannot be negative.");

            if (prospectusMandatory && prospectusAmount <= 0m)
                throw new InvalidOperationException("Prospectus amount must be greater than zero.");
        }

        private static decimal NormalizeMoney(decimal? amount)
        {
            return amount.GetValueOrDefault();
        }

        private static string? NormalizeText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private async Task<string> GenerateReceiptNoAsync(
     int tenantId,
     int branchId,
     CancellationToken ct)
        {
            var utcNow = DateTime.UtcNow;
            var today = utcNow.Date;

            var count = await _db.RegistrationFeeReceipts.CountAsync(
                x => x.TenantId == tenantId &&
                     x.BranchId == branchId &&
                     x.ReceiptDate >= today &&
                     x.ReceiptDate < today.AddDays(1),
                ct);

            return $"RCP-{utcNow:yyyyMMdd}-{count + 1:D4}";
        }




        public async Task CancelReceiptAsync(
    int tenantId,
    int branchId,
    int receiptId,
    string actor,
    CancelRegistrationReceiptRequest request,
    CancellationToken ct)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Reason))
                throw new InvalidOperationException("Cancel reason is required.");

            await using var transaction = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                var receipt = await _db.RegistrationFeeReceipts
                    .FirstOrDefaultAsync(x =>
                        x.Id == receiptId &&
                        x.TenantId == tenantId &&
                        x.BranchId == branchId,
                        ct);

                if (receipt is null)
                    throw new KeyNotFoundException("Receipt not found.");

                if (receipt.IsCancelled || receipt.ReceiptStatus == RegistrationReceiptStatus.Cancelled)
                    throw new InvalidOperationException("Receipt is already cancelled.");

                if (receipt.IsRefunded || receipt.ReceiptStatus == RegistrationReceiptStatus.Refunded)
                    throw new InvalidOperationException("Refunded receipt cannot be cancelled.");

                var registration = await _db.StudentRegistrations
                    .FirstOrDefaultAsync(x =>
                        x.Id == receipt.RegistrationId &&
                        x.TenantId == tenantId &&
                        x.BranchId == branchId,
                        ct);

                receipt.IsCancelled = true;
                receipt.ReceiptStatus = RegistrationReceiptStatus.Cancelled;
                receipt.CancelReason = request.Reason.Trim();
                receipt.CancelledOn = DateTime.UtcNow;
                receipt.CancelledBy = string.IsNullOrWhiteSpace(actor) ? "System" : actor.Trim();

                if (registration is not null)
                {
                    registration.FeePaid = false;
                    registration.PaymentStatus = RegistrationPaymentStatus.Cancelled;
                    registration.Status = RegistrationStatus.Pending;
                }

                await AddAuditAsync(
                    tenantId,
                    branchId,
                    receipt.Id,
                    "Cancelled",
                    request.Reason,
                    receipt.TotalAmount,
                    actor,
                    ct);

                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }


        public async Task RefundReceiptAsync(
    int tenantId,
    int branchId,
    int receiptId,
    string actor,
    RefundRegistrationReceiptRequest request,
    CancellationToken ct)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Amount <= 0)
                throw new InvalidOperationException("Refund amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new InvalidOperationException("Refund reason is required.");

            await using var transaction = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                var receipt = await _db.RegistrationFeeReceipts
                    .FirstOrDefaultAsync(x =>
                        x.Id == receiptId &&
                        x.TenantId == tenantId &&
                        x.BranchId == branchId,
                        ct);

                if (receipt is null)
                    throw new KeyNotFoundException("Receipt not found.");

                if (receipt.IsCancelled || receipt.ReceiptStatus == RegistrationReceiptStatus.Cancelled)
                    throw new InvalidOperationException("Cancelled receipt cannot be refunded.");

                if (receipt.IsRefunded || receipt.ReceiptStatus == RegistrationReceiptStatus.Refunded)
                    throw new InvalidOperationException("Receipt is already refunded.");

                if (request.Amount > receipt.TotalAmount)
                    throw new InvalidOperationException("Refund amount cannot exceed receipt total amount.");

                var registration = await _db.StudentRegistrations
                    .FirstOrDefaultAsync(x =>
                        x.Id == receipt.RegistrationId &&
                        x.TenantId == tenantId &&
                        x.BranchId == branchId,
                        ct);

                receipt.IsRefunded = true;
                receipt.ReceiptStatus = RegistrationReceiptStatus.Refunded;
                receipt.RefundedAmount = request.Amount;
                receipt.RefundReason = request.Reason.Trim();
                receipt.RefundedOn = DateTime.UtcNow;
                receipt.RefundedBy = string.IsNullOrWhiteSpace(actor) ? "System" : actor.Trim();

                if (registration is not null)
                {
                    registration.FeePaid = false;
                    registration.PaymentStatus = RegistrationPaymentStatus.Refunded;
                    registration.Status = RegistrationStatus.Pending;
                }

                await AddAuditAsync(
                    tenantId,
                    branchId,
                    receipt.Id,
                    "Refunded",
                    request.Reason,
                    request.Amount,
                    actor,
                    ct);

                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        private async Task AddAuditAsync(
    int tenantId,
    int branchId,
    int receiptId,
    string action,
    string? reason,
    decimal? amount,
    string actor,
    CancellationToken ct)
        {
            var audit = new RegistrationFeeReceiptAudit
            {
                TenantId = tenantId,
                BranchId = branchId,
                ReceiptId = receiptId,
                Action = action,
                Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
                Amount = amount,
                PerformedBy = string.IsNullOrWhiteSpace(actor) ? "System" : actor.Trim(),
                PerformedOn = DateTime.UtcNow
            };

            await _db.RegistrationFeeReceiptAudits.AddAsync(audit, ct);
        }

        private sealed class FeeComputation
        {
            public decimal RegistrationAmount { get; set; }
            public bool IncludeProspectus { get; set; }
            public decimal ProspectusAmount { get; set; }
            public string? ProspectusLabel { get; set; }
            public decimal TotalAmount { get; set; }
        }




    }
}