using Shala.Domain.Common;
using Shala.Domain.Entities.Students;

namespace Shala.Domain.Entities.Academics;

public class AcademicYear : AuditableEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StudentAdmission> StudentAdmissions { get; set; } = new List<StudentAdmission>();
}