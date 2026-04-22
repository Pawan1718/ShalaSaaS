using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Registration
{
    public class ConvertRegistrationRequest
    {
        public int? AcademicYearId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Class must be greater than 0.")]
        public int? ClassId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Section must be greater than 0.")]
        public int? SectionId { get; set; }

        public DateTime? AdmissionDate { get; set; }

        [MaxLength(50, ErrorMessage = "Roll number cannot exceed 50 characters.")]
        public string? RollNo { get; set; }
    }
}