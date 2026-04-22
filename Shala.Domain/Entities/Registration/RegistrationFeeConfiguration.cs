using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shala.Domain.Common;

namespace Shala.Domain.Entities.Registration
{
    [Table("RegistrationFeeConfigurations")]
    public class RegistrationFeeConfiguration : AuditableEntity, ITenantEntity, IBranchEntity
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public bool IsRegistrationModuleEnabled { get; set; } = true;

        [Range(0, 9999999.99)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RegistrationFeeAmount { get; set; }

        [Required]
        public bool IsRegistrationFeeMandatory { get; set; } = true;

        public int? RegistrationFeeHeadId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}