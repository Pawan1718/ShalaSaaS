namespace Shala.Domain.Entities.Academics;

public class AdmissionNumberCounter
{
    public int Id { get; set; }

    public int TenantId { get; set; }
    public int BranchId { get; set; }
    public int AcademicYearId { get; set; }

    public int LastNumber { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}