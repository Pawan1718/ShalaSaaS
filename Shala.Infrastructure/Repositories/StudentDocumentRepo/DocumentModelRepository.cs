using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Infrastructure.Data;
using Shala.Infrastructure.Repositories;

namespace Shala.Infrastructure.Repositories.StudentDocumentRepo
{
    public sealed class DocumentModelRepository
        : GenericRepository<DocumentModel>, IDocumentModelRepository
    {
        public DocumentModelRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<DocumentModel?> GetByCodeAsync(
            string code,
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _table.FirstOrDefaultAsync(
                x => x.Code == code && x.TenantId == tenantId && x.BranchId == branchId,
                cancellationToken);
        }

        public async Task<List<DocumentModel>> GetActiveAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _table
                .Where(x => x.TenantId == tenantId && x.BranchId == branchId && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}