using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Shared.Responses.StudentDocument
{
    public class StudentDocumentSuggestionResponse
    {
        public int Id { get; set; }
        public string SuggestionType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? SuggestedValue { get; set; }
        public decimal? ConfidenceScore { get; set; }
        public bool IsApplied { get; set; }
        public bool IsDismissed { get; set; }
    }
}
