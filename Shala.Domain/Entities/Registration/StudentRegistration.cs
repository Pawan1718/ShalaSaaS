using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shala.Domain.Common;
using Shala.Domain.Enums;

namespace Shala.Domain.Entities.Registration
{
    [Table("StudentRegistrations")]
    public class StudentRegistration : AuditableEntity, ITenantEntity, IBranchEntity
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(30)]
        public string RegistrationNo { get; set; } = string.Empty;

        [Required]
        public DateTime RegistrationDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        [Required]
        [MaxLength(150)]
        public string GuardianName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [Required]
        public int InterestedClassId { get; set; }

        [Required]
        public bool FeePaid { get; set; } = false;

        [Required]
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

        [Required]
        public bool IsDeleted { get; set; } = false;

        public int? StudentId { get; set; }

        public int? StudentAdmissionId { get; set; }

        public DateTime? ConvertedOn { get; set; }

        [MaxLength(100)]
        public string? ConvertedBy { get; set; }
    }
}