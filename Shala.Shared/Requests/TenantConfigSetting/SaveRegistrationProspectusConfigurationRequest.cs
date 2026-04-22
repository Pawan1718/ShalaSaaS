using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.TenantConfigSetting
{
    public class SaveRegistrationProspectusConfigurationRequest
    {
        public bool IncludeProspectus { get; set; } = false;

        [Range(0, 9999999.99, ErrorMessage = "Prospectus amount must be between 0 and 9999999.99.")]
        public decimal ProspectusAmount { get; set; } = 0m;

        public bool IsProspectusMandatory { get; set; } = false;

        public int? ProspectusFeeHeadId { get; set; }

        [MaxLength(100, ErrorMessage = "Prospectus display name cannot exceed 100 characters.")]
        public string? ProspectusDisplayName { get; set; }

        public bool ShowProspectusInReceipt { get; set; } = true;

        public bool IsActive { get; set; } = true;
    }
}