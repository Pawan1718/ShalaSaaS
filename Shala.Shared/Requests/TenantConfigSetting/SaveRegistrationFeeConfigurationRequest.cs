using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.TenantConfigSetting
{
    public class SaveRegistrationFeeConfigurationRequest
    {
        [Range(0, 9999999.99, ErrorMessage = "Registration fee amount must be between 0 and 9999999.99.")]
        public decimal RegistrationFeeAmount { get; set; }

        public bool IsRegistrationFeeMandatory { get; set; } = true;

        public int? RegistrationFeeHeadId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsRegistrationModuleEnabled { get; set; } = true;

    }
}