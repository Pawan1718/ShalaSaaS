namespace Shala.Shared.Requests.Students;

public class UpdateStudentGuardiansRequest
{
    public int StudentId { get; set; }
    public List<UpdateGuardianItemRequest> Guardians { get; set; } = [];
}

public class UpdateGuardianItemRequest
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RelationType { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Occupation { get; set; }
    public string? Address { get; set; }
    public bool IsPrimary { get; set; }
}