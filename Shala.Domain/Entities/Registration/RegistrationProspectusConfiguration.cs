using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shala.Domain.Common;

namespace Shala.Domain.Entities.Registration
{
    [Table("RegistrationProspectusConfigurations")]
    public class RegistrationProspectusConfiguration : AuditableEntity, ITenantEntity, IBranchEntity
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public bool IncludeProspectus { get; set; } = false;

        [Range(0, 9999999.99)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProspectusAmount { get; set; } = 0m;

        [Required]
        public bool IsProspectusMandatory { get; set; } = false;

        public int? ProspectusFeeHeadId { get; set; }

        [MaxLength(100)]
        public string? ProspectusDisplayName { get; set; }

        [Required]
        public bool ShowProspectusInReceipt { get; set; } = true;

        [Required]
        public bool IsActive { get; set; } = true;
    }
}