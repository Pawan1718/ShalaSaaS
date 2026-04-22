using Shala.Domain.Common;
using Shala.Domain.Entities.StudentDocuments;

namespace Shala.Domain.Entities.StudentDocuments
{
    public class StudentDocument : AuditableEntity, ITenantEntity, IBranchEntity
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }

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

        public bool IsRequired { get; set; } = false;
        public bool IsVerified { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Uploaded";

        public string? Remarks { get; set; }

        public virtual DocumentModel? DocumentModel { get; set; }
        public virtual Students.Student? Student { get; set; }
        public virtual StudentDocumentAnalysis? Analysis { get; set; }
        public virtual ICollection<StudentDocumentSuggestion> Suggestions { get; set; } = new List<StudentDocumentSuggestion>();
    }
}