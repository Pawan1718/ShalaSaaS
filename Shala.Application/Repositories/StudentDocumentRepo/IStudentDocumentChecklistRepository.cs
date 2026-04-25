using Shala.Application.Common;
using Shala.Domain.Entities.StudentDocuments;

namespace Shala.Application.Repositories.StudentDocumentRepo;

public interface IStudentDocumentChecklistRepository : IGenericRepository<StudentDocumentChecklist>
{
    Task<List<StudentDocumentChecklist>> GetByAdmissionAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default);


}