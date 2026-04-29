namespace Shala.Web.Models.Fees;

public sealed class FeeStructureFormVm
{
    public int Id { get; set; }

    public int AcademicYearId { get; set; }
    public int AcademicClassId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public List<FeeStructureItemFormVm> Items { get; set; } = new();
}