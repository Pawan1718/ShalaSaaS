using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.StudentDocumentRepo;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Infrastructure.Data;
using Shala.Infrastructure.Repositories;

namespace Shala.Infrastructure.Repositories.StudentDocumentRepo;

public sealed class StudentDocumentChecklistRepository
    : GenericRepository<StudentDocumentChecklist>, IStudentDocumentChecklistRepository
{
    public StudentDocumentChecklistRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<List<StudentDocumentChecklist>> GetByAdmissionAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.StudentAdmissionId == studentAdmissionId &&
                x.IsActive)
            .ToListAsync(cancellationToken);
    }
}