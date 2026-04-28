namespace Shala.Shared.Responses.TenantConfigSetting;

public class RollNumberSettingResponse
{
    public int TenantId { get; set; }

    public bool AutoGenerate { get; set; }
    public bool AllowManualOverride { get; set; }

    public bool ResetPerAcademicYear { get; set; }
    public bool ResetPerClass { get; set; }
    public bool ResetPerSection { get; set; }

    public int StartFrom { get; set; }
    public int NumberPadding { get; set; }

    public string Prefix { get; set; } = string.Empty;
    public string Format { get; set; } = "{number}";
}