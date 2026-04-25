namespace Shala.Shared.Responses.StudentDocument;

public sealed class StudentDocumentChecklistResponse
{
    public int StudentAdmissionId { get; set; }

    public int TotalDocuments { get; set; }
    public int RequiredDocuments { get; set; }
    public int ReceivedDocuments { get; set; }
    public int PendingRequiredDocuments { get; set; }

    public bool IsValid { get; set; }

    public List<StudentDocumentChecklistItemResponse> Items { get; set; } = new();
}

public sealed class StudentDocumentChecklistItemResponse
{
    public int DocumentModelId { get; set; }

    public string DocumentName { get; set; } = string.Empty;
    public string DocumentCode { get; set; } = string.Empty;
    public string? Description { get; set; }

    public bool IsRequired { get; set; }
    public bool IsReceived { get; set; }

    public string? Remark { get; set; }
    public DateTime? ReceivedDate { get; set; }

    public int DisplayOrder { get; set; }
}