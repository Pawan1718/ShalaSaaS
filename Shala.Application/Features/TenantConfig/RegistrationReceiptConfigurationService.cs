using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Registration;
using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Application.Features.TenantConfig
{
    public class RegistrationReceiptConfigurationService : IRegistrationReceiptConfigurationService
    {
        private readonly IRegistrationReceiptConfigurationRepository _repo;

        public RegistrationReceiptConfigurationService(IRegistrationReceiptConfigurationRepository repo)
        {
            _repo = repo;
        }

        public async Task<RegistrationReceiptConfigurationResponse?> GetAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetActiveAsync(tenantId, branchId, cancellationToken);

            if (entity == null)
                return null;

            return Map(entity);
        }

        public async Task<RegistrationReceiptConfigurationResponse> SaveAsync(
            int tenantId,
            int branchId,
            SaveRegistrationReceiptConfigurationRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetByScopeAsync(tenantId, branchId, cancellationToken);

            if (entity == null)
            {
                entity = new RegistrationReceiptConfiguration
                {
                    TenantId = tenantId,
                    BranchId = branchId
                };

                await _repo.AddAsync(entity, cancellationToken);
            }

            entity.AllowPrintReceipt = request.AllowPrintReceipt;
            entity.AllowDownloadReceipt = request.AllowDownloadReceipt;
            entity.AutoPrintAfterSave = request.AutoPrintAfterSave;
            entity.ReceiptTitle = string.IsNullOrWhiteSpace(request.ReceiptTitle)
                ? null
                : request.ReceiptTitle.Trim();
            entity.ReceiptFooterNote = string.IsNullOrWhiteSpace(request.ReceiptFooterNote)
                ? null
                : request.ReceiptFooterNote.Trim();
            entity.ShowStudentDetailsInReceipt = request.ShowStudentDetailsInReceipt;
            entity.ShowFeeHeadInReceipt = request.ShowFeeHeadInReceipt;
            entity.ShowAmountInWords = request.ShowAmountInWords;
            entity.IsActive = request.IsActive;

            await _repo.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        private static RegistrationReceiptConfigurationResponse Map(RegistrationReceiptConfiguration entity)
        {
            return new RegistrationReceiptConfigurationResponse
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                BranchId = entity.BranchId,
                AllowPrintReceipt = entity.AllowPrintReceipt,
                AllowDownloadReceipt = entity.AllowDownloadReceipt,
                AutoPrintAfterSave = entity.AutoPrintAfterSave,
                ReceiptTitle = entity.ReceiptTitle,
                ReceiptFooterNote = entity.ReceiptFooterNote,
                ShowStudentDetailsInReceipt = entity.ShowStudentDetailsInReceipt,
                ShowFeeHeadInReceipt = entity.ShowFeeHeadInReceipt,
                ShowAmountInWords = entity.ShowAmountInWords,
                IsActive = entity.IsActive
            };
        }
    }
}