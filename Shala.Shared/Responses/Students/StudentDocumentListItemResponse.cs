namespace Shala.Shared.Responses.Students;

public class StudentDocumentListItemResponse
{
    public int Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? FileSize { get; set; }
    public bool IsVerified { get; set; }
}