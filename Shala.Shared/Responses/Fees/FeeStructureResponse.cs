namespace Shala.Shared.Responses.Fees;

public class FeeStructureResponse
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    public int AcademicYearId { get; set; }
    public int AcademicClassId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public List<FeeStructureItemResponse> Items { get; set; } = new();
}