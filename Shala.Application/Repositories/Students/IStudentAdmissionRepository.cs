using Shala.Domain.Entities.Academics;
using Shala.Domain.Entities.Students;
using Shala.Shared.Responses.Students;

public interface IStudentAdmissionRepository
{
    Task<bool> StudentExistsAsync(
        int studentId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    Task<AcademicYear?> GetAcademicYearAsync(
        int academicYearId,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<AcademicClass?> GetAcademicClassAsync(
        int classId,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<Section?> GetSectionAsync(
        int sectionId,
        int tenantId,
        int branchId,
        int classId,
        CancellationToken cancellationToken = default);

    Task<bool> AdmissionExistsAsync(
        int studentId,
        int tenantId,
        int academicYearId,
        CancellationToken cancellationToken = default);

    Task<int> GetAdmissionCountAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        CancellationToken cancellationToken = default);

    Task AddAdmissionAsync(
        StudentAdmission admission,
        CancellationToken cancellationToken = default);

    Task<StudentAdmission?> GetAdmissionByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);

    void UpdateAdmission(StudentAdmission admission);

    void DeleteAdmission(StudentAdmission admission);

    Task<bool> RollNoExistsAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        string rollNo,
        int? excludeAdmissionId,
        CancellationToken cancellationToken = default);

    Task<int> GetRollNumberRunningCountAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        bool resetPerAcademicYear,
        bool resetPerClass,
        bool resetPerSection,
        int? excludeAdmissionId,
        CancellationToken cancellationToken = default);


   
 



    Task<int> GetSectionAdmissionCountAsync(
    int tenantId,
    int branchId,
    int academicYearId,
    int classId,
    int sectionId,
    CancellationToken cancellationToken = default);

    Task<List<string>> GetAssignedRollNumbersAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        int? excludeAdmissionId,
        CancellationToken cancellationToken = default);

    Task<List<StudentAdmissionListItemResponse>> GetAdmissionsByAcademicYearAndClassAsync(
    int tenantId,
    int branchId,
    int academicYearId,
    int classId,
    CancellationToken cancellationToken = default);



}