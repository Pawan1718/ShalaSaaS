namespace Shala.Application.Common
{
    public interface IBranchCodeGenerator
    {
        Task<string> GenerateAsync(int tenantId, string branchName, CancellationToken cancellationToken = default);
    }
}