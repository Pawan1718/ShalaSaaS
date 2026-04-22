using Shala.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Domain.Entities.StudentDocuments
{
    public class StudentDocumentFieldMatch : AuditableEntity, ITenantEntity, IBranchEntity
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public int StudentDocumentAnalysisId { get; set; }

        public string FieldName { get; set; } = string.Empty;

        public string? DocumentValue { get; set; }
        public string? FormValue { get; set; }

        public string MatchStatus { get; set; } = "Pending";
        // Matched, PartialMatch, Mismatch, MissingInForm, MissingInDocument

        public decimal? ConfidenceScore { get; set; }

        public bool IsCritical { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public string? Suggestion { get; set; }

        public virtual StudentDocumentAnalysis StudentDocumentAnalysis { get; set; } = null!;
    }
}
