using Shala.Shared.Requests.StudentDocument;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument;

public interface IStudentDocumentChecklistService
{
    Task<StudentDocumentChecklistResponse> GetByAdmissionAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default);

    Task<StudentDocumentChecklistResponse> SaveAsync(
        int tenantId,
        int branchId,
        string actor,
        SaveStudentDocumentChecklistRequest request,
        CancellationToken cancellationToken = default);

    Task<StudentDocumentChecklistResponse> ValidateAsync(
        int tenantId,
        int branchId,
        int studentAdmissionId,
        CancellationToken cancellationToken = default);


}