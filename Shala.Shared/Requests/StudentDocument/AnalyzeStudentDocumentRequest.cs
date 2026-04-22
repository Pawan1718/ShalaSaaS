namespace Shala.Shared.Requests.StudentDocument
{
    public class AnalyzeStudentDocumentRequest
    {
        public int StudentDocumentId { get; set; }
        public bool ForceReAnalyze { get; set; } = false;
    }
}