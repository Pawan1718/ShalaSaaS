namespace Shala.Shared.Responses.Registration
{
    public class RegistrationListItemResponse
    {
        public int Id { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public int InterestedClassId { get; set; }

        public bool FeePaid { get; set; }
        public string Status { get; set; } = string.Empty;

        public int? ReceiptId { get; set; }
        public string? ReceiptNo { get; set; }
    }
}