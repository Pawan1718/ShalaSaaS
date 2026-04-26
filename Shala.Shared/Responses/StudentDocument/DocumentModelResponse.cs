namespace Shala.Shared.Responses.StudentDocument;

public sealed class DocumentModelResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public bool IsRequired { get; set; }
    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }
}