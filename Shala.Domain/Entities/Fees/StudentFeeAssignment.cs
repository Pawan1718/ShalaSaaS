using Shala.Domain.Common;
using Shala.Domain.Entities.Students;

namespace Shala.Domain.Entities.Fees;

public class StudentFeeAssignment : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }
    public int FeeStructureId { get; set; }

    public decimal DiscountAmount { get; set; }
    public decimal AdditionalChargeAmount { get; set; }

    public bool IsActive { get; set; } = true;

    public Student Student { get; set; } = default!;
    public StudentAdmission StudentAdmission { get; set; } = default!;
    public FeeStructure FeeStructure { get; set; } = default!;

    public ICollection<StudentCharge> Charges { get; set; } = new List<StudentCharge>();
}