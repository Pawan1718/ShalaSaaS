using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Students;

public class UpdateStudentRequest
{

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Range(1, 3)]
    public int Gender { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? AadhaarNo { get; set; }

    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Mobile { get; set; }

    [EmailAddress]
    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    [Range(1, 5)]
    public int Status { get; set; }


}