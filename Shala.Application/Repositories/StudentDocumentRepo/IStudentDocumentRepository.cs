using Shala.Application.Common;
using Shala.Domain.Entities.StudentDocuments;

namespace Shala.Application.Repositories.StudentDocumentRepo
{
    public interface IStudentDocumentRepository : IGenericRepository<StudentDocument>
    {
        Task<StudentDocument?> GetScopedByIdAsync(
            int id,
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<List<StudentDocument>> GetByStudentIdAsync(
            int studentId,
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default);
    }
}