using Shala.Domain.Common;

namespace Shala.Domain.Entities.Fees;

public class FeeHead : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public string Name { get; set; } = string.Empty;     // Admission Fee, Tuition Fee
    public string Code { get; set; } = string.Empty;     // ADMISSION, TUITION
    public string? Description { get; set; }


    public bool IsRegistrationFee { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public ICollection<FeeStructureItem> FeeStructureItems { get; set; } = new List<FeeStructureItem>();
    public ICollection<StudentCharge> StudentCharges { get; set; } = new List<StudentCharge>();
}