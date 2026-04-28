namespace Shala.Shared.Requests.StudentDocument;

public sealed class CreateDocumentModelRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public bool IsRequired { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}