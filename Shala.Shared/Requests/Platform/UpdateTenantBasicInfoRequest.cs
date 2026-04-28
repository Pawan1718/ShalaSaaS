using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Platform
{
    public class UpdateTenantBasicInfoRequest
    {
        [Required]
        [StringLength(150)]
        public string SchoolName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SubscriptionPlan { get; set; } = string.Empty;
    }
}