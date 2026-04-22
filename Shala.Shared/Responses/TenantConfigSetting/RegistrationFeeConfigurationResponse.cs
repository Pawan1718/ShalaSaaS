namespace Shala.Shared.Responses.TenantConfigSetting
{
    public class RegistrationFeeConfigurationResponse
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public decimal RegistrationFeeAmount { get; set; }
        public bool IsRegistrationFeeMandatory { get; set; }
        public int? RegistrationFeeHeadId { get; set; }

        public bool IsActive { get; set; }

        public bool IsRegistrationModuleEnabled { get; set; }

    }
}