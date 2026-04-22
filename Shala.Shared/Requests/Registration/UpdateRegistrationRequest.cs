using System.ComponentModel.DataAnnotations;
using Shala.Domain.Enums;

namespace Shala.Shared.Requests.Registration
{
    public class UpdateRegistrationRequest
    {
        [Required(ErrorMessage = "Registration date is required.")]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Middle name cannot exceed 100 characters.")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "Guardian name is required.")]
        [MaxLength(150, ErrorMessage = "Guardian name cannot exceed 150 characters.")]
        public string GuardianName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

        [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
        public string? Note { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Interested class is required.")]
        public int InterestedClassId { get; set; }
    }
}