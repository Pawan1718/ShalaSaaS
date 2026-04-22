using Shala.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Domain.Entities.StudentDocuments
{
    public class StudentDocumentSimilarity : AuditableEntity, ITenantEntity, IBranchEntity
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public int SourceDocumentId { get; set; }
        public int ComparedDocumentId { get; set; }

        public decimal SimilarityScore { get; set; }
        public string SimilarityType { get; set; } = string.Empty;
        // Duplicate, SupportingDocument, SameType, CrossValidation

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
