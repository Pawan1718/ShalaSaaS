using System.ComponentModel.DataAnnotations;

namespace Shala.Web.Models.Auth
{
    public class TenantLoginVm
    {
        [Required]
        public string UserIdOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}