using Shala.Domain.Common;

namespace Shala.Domain.Entities.Students;

public class RollNumberSetting : AuditableEntity, ITenantEntity
{
    public int TenantId { get; set; }

    public bool AutoGenerate { get; set; } = true;
    public bool AllowManualOverride { get; set; } = false;

    public bool ResetPerAcademicYear { get; set; } = true;
    public bool ResetPerClass { get; set; } = true;
    public bool ResetPerSection { get; set; } = false;

    public int StartFrom { get; set; } = 1;
    public int NumberPadding { get; set; } = 3;

    public string Prefix { get; set; } = string.Empty;
    public string Format { get; set; } = "{number}";
}