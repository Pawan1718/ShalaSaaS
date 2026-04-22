using Shala.Application.Common;
using Shala.Domain.Entities.StudentDocuments;

namespace Shala.Application.Repositories.StudentDocumentRepo
{
    public interface IDocumentModelRepository : IGenericRepository<DocumentModel>
    {
        Task<DocumentModel?> GetByCodeAsync(
            string code,
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<List<DocumentModel>> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);
    }
}