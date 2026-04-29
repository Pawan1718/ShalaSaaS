using Shala.Domain.Common;
using Shala.Domain.Enums;

namespace Shala.Domain.Entities.Fees;

public class FeeStructureItem : AuditableEntity
{
    public int FeeStructureId { get; set; }
    public int FeeHeadId { get; set; }

    public string Label { get; set; } = string.Empty;    // Tuition, Annual Fee, Math Fee
    public decimal Amount { get; set; }

    public FeeFrequencyType FrequencyType { get; set; } = FeeFrequencyType.OneTime;

    public int? StartMonth { get; set; }   // 1 to 12
    public int? EndMonth { get; set; }     // 1 to 12
    public int? DueDay { get; set; }       // Example 10th of month


    public FeeApplyType ApplyType { get; set; }
    public bool IsOptional { get; set; }
    public bool IsActive { get; set; } = true;

    public FeeStructure FeeStructure { get; set; } = default!;
    public FeeHead FeeHead { get; set; } = default!;
}