namespace Shala.Shared.Responses.TenantConfigSetting
{
    public class RegistrationProspectusConfigurationResponse
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public bool IncludeProspectus { get; set; }
        public decimal ProspectusAmount { get; set; }
        public bool IsProspectusMandatory { get; set; }

        public int? ProspectusFeeHeadId { get; set; }
        public string? ProspectusDisplayName { get; set; }

        public bool ShowProspectusInReceipt { get; set; }
        public bool IsActive { get; set; }
    }
}