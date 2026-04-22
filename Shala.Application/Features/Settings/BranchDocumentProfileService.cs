using Shala.Application.Repositories.Settings;
using Shala.Domain.Entities.Settings;
using Shala.Shared.Requests.Settings;
using Shala.Shared.Responses.Settings;

namespace Shala.Application.Features.Settings
{
    public class BranchDocumentProfileService : IBranchDocumentProfileService
    {
        private readonly IBranchDocumentProfileRepository _repo;

        public BranchDocumentProfileService(IBranchDocumentProfileRepository repo)
        {
            _repo = repo;
        }

        public async Task<BranchDocumentProfileResponse?> GetAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetActiveAsync(tenantId, branchId, cancellationToken);
            return entity is null ? null : Map(entity);
        }

        public async Task<BranchDocumentProfileResponse> SaveAsync(
            int tenantId,
            int branchId,
            SaveBranchDocumentProfileRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetByScopeAsync(tenantId, branchId, cancellationToken);

            if (entity is null)
            {
                entity = new BranchDocumentProfile
                {
                    TenantId = tenantId,
                    BranchId = branchId
                };

                await _repo.AddAsync(entity, cancellationToken);
            }

            entity.DisplayName = Normalize(request.DisplayName);
            entity.LogoUrl = Normalize(request.LogoUrl);
            entity.AddressLine = Normalize(request.AddressLine);
            entity.Phone = Normalize(request.Phone);
            entity.Email = Normalize(request.Email);
            entity.PrimaryColorHex = Normalize(request.PrimaryColorHex);
            entity.ReceiptTitle = Normalize(request.ReceiptTitle);
            entity.ReceiptFooterNote = Normalize(request.ReceiptFooterNote);
            entity.SignatureLabel = Normalize(request.SignatureLabel);

            entity.ShowLogo = request.ShowLogo;
            entity.ShowAddress = request.ShowAddress;
            entity.ShowContactInfo = request.ShowContactInfo;
            entity.ShowStudentDetails = request.ShowStudentDetails;
            entity.ShowFeeBreakup = request.ShowFeeBreakup;
            entity.ShowAmountInWords = request.ShowAmountInWords;
            entity.ShowSignature = request.ShowSignature;

            entity.AllowPrintReceipt = request.AllowPrintReceipt;
            entity.AllowDownloadReceipt = request.AllowDownloadReceipt;
            entity.AutoPrintAfterSave = request.AutoPrintAfterSave;
            entity.IsActive = request.IsActive;

            await _repo.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        private static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static BranchDocumentProfileResponse Map(BranchDocumentProfile entity)
        {
            return new BranchDocumentProfileResponse
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                BranchId = entity.BranchId,
                DisplayName = entity.DisplayName,
                LogoUrl = entity.LogoUrl,
                AddressLine = entity.AddressLine,
                Phone = entity.Phone,
                Email = entity.Email,
                PrimaryColorHex = entity.PrimaryColorHex,
                ReceiptTitle = entity.ReceiptTitle,
                ReceiptFooterNote = entity.ReceiptFooterNote,
                SignatureLabel = entity.SignatureLabel,
                ShowLogo = entity.ShowLogo,
                ShowAddress = entity.ShowAddress,
                ShowContactInfo = entity.ShowContactInfo,
                ShowStudentDetails = entity.ShowStudentDetails,
                ShowFeeBreakup = entity.ShowFeeBreakup,
                ShowAmountInWords = entity.ShowAmountInWords,
                ShowSignature = entity.ShowSignature,
                AllowPrintReceipt = entity.AllowPrintReceipt,
                AllowDownloadReceipt = entity.AllowDownloadReceipt,
                AutoPrintAfterSave = entity.AutoPrintAfterSave,
                IsActive = entity.IsActive
            };
        }
    }
}