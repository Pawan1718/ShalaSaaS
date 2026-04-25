using Shala.Domain.Common;
using Shala.Domain.Entities.Fees;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Domain.Enums;

namespace Shala.Domain.Entities.Students;

public class Student : AuditableEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? AadhaarNo { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? BloodGroup { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PhotoUrl { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;

    public ICollection<Guardian> Guardians { get; set; } = new List<Guardian>();
    public ICollection<StudentAdmission> Admissions { get; set; } = new List<StudentAdmission>();
    public ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();
    public ICollection<StudentFeeAssignment> FeeAssignments { get; set; } = new List<StudentFeeAssignment>();
    public ICollection<StudentCharge> StudentCharges { get; set; } = new List<StudentCharge>();
}