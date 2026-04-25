using Shala.Domain.Common;
using Shala.Domain.Entities.Academics;
using Shala.Domain.Entities.Fees;
using Shala.Domain.Enums;


namespace Shala.Domain.Entities.Students;

public class StudentAdmission : AuditableEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public int StudentId { get; set; }
    public int AcademicYearId { get; set; }
    public int AcademicClassId { get; set; }
    public int? SectionId { get; set; }
    public string AdmissionNo { get; set; } = string.Empty;

    public string? RollNo { get; set; }
    public DateTime AdmissionDate { get; set; }
    public AdmissionStatus Status { get; set; } = AdmissionStatus.Active;
    public bool IsCurrent { get; set; } = true;

    public Student Student { get; set; } = default!;
    public AcademicYear AcademicYear { get; set; } = default!;
    public AcademicClass AcademicClass { get; set; } = default!;
    public Section? Section { get; set; }


    public ICollection<StudentFeeAssignment> FeeAssignments { get; set; } = new List<StudentFeeAssignment>();
public ICollection<StudentCharge> StudentCharges { get; set; } = new List<StudentCharge>();
}