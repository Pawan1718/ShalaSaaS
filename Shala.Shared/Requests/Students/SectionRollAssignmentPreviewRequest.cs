namespace Shala.Shared.Requests.Students;

public class SectionRollAssignmentPreviewRequest
{
    public int StudentAdmissionId { get; set; }
    public int? SectionId { get; set; }
    public bool AutoGenerateRollNo { get; set; } = true;
    public string? RollNo { get; set; }
}