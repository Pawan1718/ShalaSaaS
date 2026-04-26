using Shala.Shared.Common;

namespace Shala.Shared.Requests.Platform
{
    public class TenantListRequest : PagedRequest
    {
        public bool? IsActive { get; set; }
        public string? SubscriptionPlan { get; set; }
        public string? BusinessCategory { get; set; }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}