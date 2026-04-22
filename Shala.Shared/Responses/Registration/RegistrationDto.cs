namespace Shala.Shared.Responses.Registration
{
    public class RegistrationDto
    {
        public int Id { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;

        public string? InterestedClassName { get; set; }
        public int InterestedClassId { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        public string GuardianName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Note { get; set; }

        public bool FeePaid { get; set; }
        public string Status { get; set; } = string.Empty;

        public int? StudentId { get; set; }
        public int? StudentAdmissionId { get; set; }
        public DateTime? ConvertedOn { get; set; }

        public int? LatestReceiptId { get; set; }
        public string? LatestReceiptNo { get; set; }

        public string FullName =>
            string.Join(" ", new[] { FirstName, MiddleName, LastName }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        public string ClassDisplay =>
            string.IsNullOrWhiteSpace(InterestedClassName) ? "-" : InterestedClassName;
    }
}