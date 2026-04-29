namespace Shala.Shared.Requests.Students;

public class SaveRollNumberSettingRequest
{

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