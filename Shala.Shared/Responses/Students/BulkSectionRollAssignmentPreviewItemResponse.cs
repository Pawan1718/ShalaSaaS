namespace Shala.Shared.Responses.Students;

public class BulkSectionRollAssignmentPreviewItemResponse
{
    public int StudentAdmissionId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }

    public string? CurrentSectionName { get; set; }
    public string? CurrentRollNo { get; set; }

    public int? NewSectionId { get; set; }
    public string? NewSectionName { get; set; }
    public string? NewRollNo { get; set; }

    public bool HasConflict { get; set; }
    public string? Message { get; set; }
}