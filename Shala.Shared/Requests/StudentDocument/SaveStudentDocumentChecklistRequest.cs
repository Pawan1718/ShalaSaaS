namespace Shala.Shared.Requests.StudentDocument;

public sealed class SaveStudentDocumentChecklistRequest
{
    public int StudentAdmissionId { get; set; }

    public List<SaveStudentDocumentChecklistItemRequest> Items { get; set; } = new();
}

public sealed class SaveStudentDocumentChecklistItemRequest
{
    public int DocumentModelId { get; set; }

    public bool IsReceived { get; set; }

    public string? Remark { get; set; }
}