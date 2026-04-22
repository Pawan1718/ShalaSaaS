namespace Shala.Shared.Responses.Registration
{
    public class ConvertRegistrationResponse
    {
        public int RegistrationId { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public int StudentAdmissionId { get; set; }
        public string AdmissionNo { get; set; } = string.Empty;
        public DateTime ConvertedOn { get; set; }
    }
}