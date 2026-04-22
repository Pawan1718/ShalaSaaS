using Shala.Domain.Common;
using Shala.Domain.Entities.Students;

namespace Shala.Domain.Entities.Academics;

public class Section : AuditableEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }
    public int AcademicClassId { get; set; }

    public string Name { get; set; } = string.Empty;
    public int? Capacity { get; set; }
    public bool IsActive { get; set; } = true;

    public AcademicClass AcademicClass { get; set; } = default!;

    public ICollection<StudentAdmission> StudentAdmissions { get; set; } = new List<StudentAdmission>();
}