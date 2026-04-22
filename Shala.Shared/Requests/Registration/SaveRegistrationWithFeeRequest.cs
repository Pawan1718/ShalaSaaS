using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Registration
{
    public class SaveRegistrationWithFeeRequest
    {
        [Required(ErrorMessage = "Registration details are required.")]
        public CreateRegistrationRequest Registration { get; set; } = new();

        [Required(ErrorMessage = "Fee details are required.")]
        public CollectRegistrationFeeRequest Fee { get; set; } = new();
    }
}