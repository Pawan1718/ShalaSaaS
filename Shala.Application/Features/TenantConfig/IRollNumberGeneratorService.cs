namespace Shala.Application.Features.TenantConfig;

public interface IRollNumberGeneratorService
{
    Task<string?> ResolveRollNoForCreateAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        string? requestedRollNo,
        CancellationToken cancellationToken = default);

    Task<string?> ResolveRollNoForUpdateAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        int admissionId,
        int oldAcademicYearId,
        int oldClassId,
        int? oldSectionId,
        string? requestedRollNo,
        CancellationToken cancellationToken = default);
}