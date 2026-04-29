namespace Shala.Shared.Responses.Academics;

public class AcademicClassListItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int Sequence { get; set; }
    public bool IsActive { get; set; }
}