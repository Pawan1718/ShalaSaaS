namespace Shala.Shared.Responses.Students;

public class SectionRollAssignmentPreviewResponse
{
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }

    public int CurrentStrength { get; set; }
    public int? Capacity { get; set; }
    public int AvailableSeats { get; set; }

    public string? NextSuggestedRollNo { get; set; }
    public bool RollNoAlreadyExists { get; set; }

    public string? Message { get; set; }
}