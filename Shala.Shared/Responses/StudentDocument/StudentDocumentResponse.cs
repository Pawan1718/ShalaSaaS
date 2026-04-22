namespace Shala.Shared.Responses.StudentDocument
{
    public class StudentDocumentResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int? StudentRegistrationId { get; set; }
        public int? StudentAdmissionId { get; set; }

        public int? DocumentModelId { get; set; }

        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        public bool IsRequired { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}