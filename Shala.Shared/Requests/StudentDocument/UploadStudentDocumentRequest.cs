using Microsoft.AspNetCore.Http;

namespace Shala.Shared.Requests.StudentDocument
{
    public class UploadStudentDocumentRequest
    {
        public int StudentId { get; set; }
        public int? StudentRegistrationId { get; set; }
        public int? StudentAdmissionId { get; set; }
        public int? DocumentModelId { get; set; }

        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsRequired { get; set; }

        public IFormFile File { get; set; } = null!;
    }
}