namespace Shala.Shared.Responses.Platform
{
    public class CreateTenantResponse
    {
        public string Message { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public int MainBranchId { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
    }
}