using Shala.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Domain.Entities.StudentDocuments
{
    public class StudentDocumentAnalysis : AuditableEntity, ITenantEntity, IBranchEntity
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public int StudentDocumentId { get; set; }

        public string? ExtractedText { get; set; }
        public string? ExtractedJson { get; set; }
        // parsed fields as json

        public string? DetectedDocumentType { get; set; }

        public decimal? OcrConfidence { get; set; }
        public decimal? AiConfidence { get; set; }

        public string AnalysisStatus { get; set; } = "Pending";
        // Pending, Completed, Failed, NeedsReview

        public bool IsActive { get; set; } = true;

        public virtual StudentDocument StudentDocuments { get; set; } = null!;
        public virtual ICollection<StudentDocumentFieldMatch> FieldMatches { get; set; } = new List<StudentDocumentFieldMatch>();
    }
}
