namespace Shala.Shared.Requests.StudentDocument
{
    public class VerifyStudentDocumentRequest
    {
        public int StudentDocumentId { get; set; }
        public bool IsVerified { get; set; }
        public string? Remarks { get; set; }
    }
}