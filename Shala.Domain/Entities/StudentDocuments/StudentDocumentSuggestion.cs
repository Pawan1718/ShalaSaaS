using Shala.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Domain.Entities.StudentDocuments
{
  



    public class StudentDocumentSuggestion : AuditableEntity, ITenantEntity, IBranchEntity
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public int StudentDocumentId { get; set; }

        public string SuggestionType { get; set; } = string.Empty;
        // MissingField, MismatchWarning, DuplicateDocument, SimilarDocumentSupport, CorrectionSuggestion

        public string Message { get; set; } = string.Empty;
        public string? SuggestedValue { get; set; }

        public decimal? ConfidenceScore { get; set; }

        public bool IsApplied { get; set; } = false;
        public bool IsDismissed { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public virtual StudentDocument StudentDocument { get; set; } = null!;
    }
}
