using Shala.Domain.Common;

namespace Shala.Domain.Entities.Academics;

public class AcademicYearSetting : AuditableEntity, ITenantEntity
{
    public int TenantId { get; set; }

    public int StartMonth { get; set; }
    public int StartDay { get; set; }

    public int EndMonth { get; set; }
    public int EndDay { get; set; }

    public bool AutoCreateNextYear { get; set; } = true;
    public int CreateBeforeDays { get; set; } = 30;
}