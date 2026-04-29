using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Students;
using Shala.Domain.Entities.Academics;
using Shala.Domain.Entities.Students;
using Shala.Infrastructure.Data;
using Shala.Shared.Responses.Students;

namespace Shala.Infrastructure.Repositories.Students;

public class StudentAdmissionRepository : IStudentAdmissionRepository
{
    private readonly AppDbContext _context;

    public StudentAdmissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> StudentExistsAsync(
        int studentId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Students.AnyAsync(x =>
            x.Id == studentId &&
            x.TenantId == tenantId &&
            x.BranchId == branchId,
            cancellationToken);
    }

    public async Task<bool> AdmissionExistsAsync(
        int studentId,
        int tenantId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StudentAdmissions.AnyAsync(x =>
            x.StudentId == studentId &&
            x.TenantId == tenantId &&
            x.AcademicYearId == academicYearId,
            cancellationToken);
    }

    public async Task AddAdmissionAsync(
        StudentAdmission admission,
        CancellationToken cancellationToken = default)
    {
        await _context.StudentAdmissions.AddAsync(admission, cancellationToken);
    }

    public void UpdateAdmission(StudentAdmission admission)
    {
        _context.StudentAdmissions.Update(admission);
    }

    public void DeleteAdmission(StudentAdmission admission)
    {
        _context.StudentAdmissions.Remove(admission);
    }

    public async Task<StudentAdmission?> GetAdmissionByIdAsync(
     int id,
     int tenantId,
     int branchId,
     CancellationToken cancellationToken = default)
    {
        return await _context.StudentAdmissions
            .Include(x => x.Student)
            .Include(x => x.AcademicYear)
            .Include(x => x.AcademicClass)
            .Include(x => x.Section)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == tenantId &&
                x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<int> GetAdmissionCountAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StudentAdmissions.CountAsync(x =>
            x.TenantId == tenantId &&
            x.BranchId == branchId &&
            x.AcademicYearId == academicYearId,
            cancellationToken);
    }

    public async Task<AcademicYear?> GetAcademicYearAsync(
        int id,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AcademicYears
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == tenantId &&
                x.IsActive,
                cancellationToken);
    }

    public async Task<AcademicClass?> GetAcademicClassAsync(
        int id,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AcademicClasses
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == tenantId &&
                x.IsActive,
                cancellationToken);
    }

    public async Task<Section?> GetSectionAsync(
        int id,
        int tenantId,
        int branchId,
        int classId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sections
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicClassId == classId &&
                x.IsActive,
                cancellationToken);
    }

    public async Task<int> GetRollNumberRunningCountAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        bool resetPerAcademicYear,
        bool resetPerClass,
        bool resetPerSection,
        int? excludeAdmissionId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StudentAdmissions
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId);

        if (resetPerAcademicYear)
            query = query.Where(x => x.AcademicYearId == academicYearId);

        if (resetPerClass)
            query = query.Where(x => x.AcademicClassId == classId);

        if (resetPerSection)
        {
            if (sectionId.HasValue)
                query = query.Where(x => x.SectionId == sectionId.Value);
            else
                query = query.Where(x => x.SectionId == null);
        }

        if (excludeAdmissionId.HasValue)
            query = query.Where(x => x.Id != excludeAdmissionId.Value);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> RollNoExistsAsync(
     int tenantId,
     int branchId,
     int academicYearId,
     int classId,
     int? sectionId,
     string rollNo,
     int? excludeAdmissionId,
     CancellationToken cancellationToken = default)
    {
        var query = _context.StudentAdmissions
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicYearId == academicYearId &&
                x.AcademicClassId == classId &&
                x.RollNo == rollNo);

        if (sectionId.HasValue)
            query = query.Where(x => x.SectionId == sectionId.Value);
        else
            query = query.Where(x => x.SectionId == null);

        if (excludeAdmissionId.HasValue)
            query = query.Where(x => x.Id != excludeAdmissionId.Value);

        return await query.AnyAsync(cancellationToken);
    }


    public async Task<int> GetSectionAdmissionCountAsync(
     int tenantId,
     int branchId,
     int academicYearId,
     int classId,
     int sectionId,
     CancellationToken cancellationToken = default)
    {
        return await _context.StudentAdmissions
            .AsNoTracking()
            .CountAsync(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicYearId == academicYearId &&
                x.AcademicClassId == classId &&
                x.SectionId == sectionId,
                cancellationToken);
    }


    public async Task<List<StudentAdmissionListItemResponse>> GetAdmissionsByAcademicYearAndClassAsync(
    int tenantId,
    int branchId,
    int academicYearId,
    int classId,
    CancellationToken cancellationToken = default)
    {
        return await _context.StudentAdmissions
            .AsNoTracking()
            .Include(x => x.Student)
            .Include(x => x.AcademicYear)
            .Include(x => x.AcademicClass)
            .Include(x => x.Section)
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicYearId == academicYearId &&
                x.AcademicClassId == classId)
            .OrderBy(x => x.Student.FirstName)
            .ThenBy(x => x.Student.LastName)
            .Select(x => new StudentAdmissionListItemResponse
            {
                Id = x.Id,
                StudentId = x.StudentId,
                StudentName = string.Join(" ", new[]
                {
                x.Student.FirstName,
                x.Student.MiddleName,
                x.Student.LastName
                }.Where(s => !string.IsNullOrWhiteSpace(s))),
                AdmissionNo = x.AdmissionNo,
                AcademicYear = x.AcademicYear != null ? x.AcademicYear.Name : string.Empty,
                ClassName = x.AcademicClass != null ? x.AcademicClass.Name : string.Empty,
                SectionName = x.Section != null ? x.Section.Name : null,
                RollNo = x.RollNo,
                AdmissionDate = x.AdmissionDate,
                Status = x.Status.ToString()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetAssignedRollNumbersAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        int? excludeAdmissionId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StudentAdmissions
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicYearId == academicYearId &&
                x.AcademicClassId == classId &&
                x.RollNo != null);

        if (sectionId.HasValue)
            query = query.Where(x => x.SectionId == sectionId.Value);
        else
            query = query.Where(x => x.SectionId == null);

        if (excludeAdmissionId.HasValue)
            query = query.Where(x => x.Id != excludeAdmissionId.Value);

        return await query
            .Select(x => x.RollNo!)
            .ToListAsync(cancellationToken);
    }





    public async Task<StudentAdmission?> GetByIdAsync(
      int admissionId,
      int tenantId,
      int branchId,
      CancellationToken cancellationToken = default)
    {
        return await _context.StudentAdmissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == admissionId &&
                x.TenantId == tenantId &&
                x.BranchId == branchId,
                cancellationToken);
    }
}
