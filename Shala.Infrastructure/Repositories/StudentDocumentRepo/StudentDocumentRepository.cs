using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Infrastructure.Data;
using Shala.Infrastructure.Repositories;

namespace Shala.Infrastructure.Repositories.StudentDocumentRepo
{
    public sealed class StudentDocumentRepository
        : GenericRepository<StudentDocument>, IStudentDocumentRepository
    {
        public StudentDocumentRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<StudentDocument?> GetScopedByIdAsync(
            int id,
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _table
                .Include(x => x.Analysis)
                    .ThenInclude(x => x.FieldMatches)
                .Include(x => x.Suggestions)
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.TenantId == tenantId &&
                         x.BranchId == branchId,
                    cancellationToken);
        }

        public async Task<List<StudentDocument>> GetByStudentIdAsync(
            int studentId,
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _table
                .Where(x => x.StudentId == studentId &&
                            x.TenantId == tenantId &&
                            x.BranchId == branchId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}