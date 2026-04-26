namespace Shala.Shared.Responses.Students;

public class BulkSectionRollAssignmentPreviewResponse
{
    public int TotalStudents { get; set; }
    public int CurrentStrength { get; set; }
    public int? Capacity { get; set; }
    public int AvailableSeats { get; set; }

    public List<BulkSectionRollAssignmentPreviewItemResponse> Items { get; set; } = new();
}