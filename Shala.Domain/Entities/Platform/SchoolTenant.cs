using Shala.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Shala.Domain.Entities.Platform
{
    public class SchoolTenant
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BusinessCategory { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string MobileNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string SubscriptionPlan { get; set; } = "Basic";

        [MaxLength(100)]
        public string? Subdomain { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();


    }
}