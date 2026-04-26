using Shala.Shared.Responses.Academics;

namespace Shala.Shared.Responses.Students;

public class StudentDetailsResponse
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int BranchId { get; set; }
    public string? AdmissionNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? AadhaarNo { get; set; }
    public string? BloodGroup { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PhotoUrl { get; set; }
    public string Status { get; set; } = string.Empty;

    public List<GuardianResponse> Guardians { get; set; } = [];
    public List<StudentAdmissionResponse> Admissions { get; set; } = [];
}