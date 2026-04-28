
namespace Shala.Shared.Requests.Students;

public class UploadStudentDocumentRequest
{
    public int StudentId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
}