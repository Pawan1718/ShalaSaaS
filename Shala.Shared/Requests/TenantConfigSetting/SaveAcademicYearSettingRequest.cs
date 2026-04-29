namespace Shala.Shared.Requests.TenantConfigSetting;

public class SaveAcademicYearSettingRequest
{
    public int StartMonth { get; set; }
    public int StartDay { get; set; }
    public int EndMonth { get; set; }
    public int EndDay { get; set; }
    public bool AutoCreateNextYear { get; set; } = true;
    public int CreateBeforeDays { get; set; } = 30;
}