using Shala.Domain.Common;

namespace Shala.Domain.Entities.Fees;

public class FeeStructure : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public int AcademicYearId { get; set; }
    public int AcademicClassId { get; set; }

    public string Name { get; set; } = string.Empty;     // KG1 2026-27
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<FeeStructureItem> Items { get; set; } = new List<FeeStructureItem>();
    public ICollection<StudentFeeAssignment> StudentFeeAssignments { get; set; } = new List<StudentFeeAssignment>();
}