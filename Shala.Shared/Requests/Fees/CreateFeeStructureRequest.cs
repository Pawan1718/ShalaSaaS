namespace Shala.Shared.Requests.Fees;

public class CreateFeeStructureRequest
{
    public int AcademicYearId { get; set; }
    public int AcademicClassId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public List<FeeStructureItemRequest> Items { get; set; } = new();
}