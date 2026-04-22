namespace Shala.Shared.Responses.Students;

public class GuardianResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
}