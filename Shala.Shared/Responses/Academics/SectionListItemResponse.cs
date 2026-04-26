namespace Shala.Shared.Responses.Academics;

public class SectionListItemResponse
{
    public int Id { get; set; }

    public int AcademicClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int? Capacity { get; set; }

    public int? CurrentStrength { get; set; }   // UI: kitne students hain

    public bool IsActive { get; set; }

    public string Status => IsActive ? "Active" : "Inactive"; // UI friendly

    public string DisplayName => $"{ClassName} - {Name}"; // dropdown use

    public DateTime? CreatedOn { get; set; }  // optional (audit display)
}