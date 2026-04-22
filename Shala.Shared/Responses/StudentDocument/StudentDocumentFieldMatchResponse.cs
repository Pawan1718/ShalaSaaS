using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Shared.Responses.StudentDocument
{
    public class StudentDocumentFieldMatchResponse
    {
        public string FieldName { get; set; } = string.Empty;
        public string? DocumentValue { get; set; }
        public string? FormValue { get; set; }
        public string MatchStatus { get; set; } = string.Empty;
        public decimal? ConfidenceScore { get; set; }
        public bool IsCritical { get; set; }
        public string? Suggestion { get; set; }
    }
}
