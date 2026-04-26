using Shala.Domain.Common;
using Shala.Domain.Entities.Students;


namespace Shala.Domain.Entities.Academics;

public class AcademicClass : AuditableEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int Sequence { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<StudentAdmission> StudentAdmissions { get; set; } = new List<StudentAdmission>();
}