using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Shared.Responses.StudentDocument
{
    public class StudentDocumentAnalysisResponse
    {
        public int StudentDocumentId { get; set; }
        public string? ExtractedText { get; set; }
        public string? ExtractedJson { get; set; }

        public string? DetectedDocumentType { get; set; }

        public decimal? OcrConfidence { get; set; }
        public decimal? AiConfidence { get; set; }

        public string AnalysisStatus { get; set; } = string.Empty;

        public List<StudentDocumentFieldMatchResponse> FieldMatches { get; set; } = new();
        public List<StudentDocumentSuggestionResponse> Suggestions { get; set; } = new();
    }
}
