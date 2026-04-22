using Shala.Domain.Enum;
using Shala.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Registration
{
    public class CollectRegistrationFeeRequest
    {
        [Range(0, 9999999.99, ErrorMessage = "Amount must be between 0 and 9999999.99.")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "Payment mode is required.")]
        public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;

        [MaxLength(100, ErrorMessage = "Transaction reference cannot exceed 100 characters.")]
        public string? TransactionReference { get; set; }

        [MaxLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        public bool IncludeProspectus { get; set; } = false;

        [Range(0, 9999999.99, ErrorMessage = "Prospectus amount must be between 0 and 9999999.99.")]
        public decimal? ProspectusAmount { get; set; }
    }
}