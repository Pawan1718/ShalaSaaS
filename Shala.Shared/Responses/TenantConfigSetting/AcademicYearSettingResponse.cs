namespace Shala.Shared.Responses.TenantConfigSetting;

public class AcademicYearSettingResponse
{
    public int TenantId { get; set; }
    public int StartMonth { get; set; }
    public int StartDay { get; set; }
    public int EndMonth { get; set; }
    public int EndDay { get; set; }
    public bool AutoCreateNextYear { get; set; }
    public int CreateBeforeDays { get; set; }
}