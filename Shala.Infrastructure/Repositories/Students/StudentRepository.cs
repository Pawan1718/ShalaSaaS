using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Students;
using Shala.Domain.Entities.Students;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Students;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<Student?> GetDetailsAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Include(x => x.Guardians)
            .Include(x => x.Documents)
            .Include(x => x.Admissions).ThenInclude(x => x.AcademicYear)
            .Include(x => x.Admissions).ThenInclude(x => x.AcademicClass)
            .Include(x => x.Admissions).ThenInclude(x => x.Section)
            .Include(x => x.FeeAssignments)
            .Include(x => x.StudentCharges).ThenInclude(x => x.FeeHead)
            .Include(x => x.FeeReceipts)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(
        int tenantId,
        int branchId,
        string? search,
        int? academicYearId,
        int? classId,
        int? sectionId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Students
            .AsNoTracking()
            .Include(x => x.Admissions).ThenInclude(x => x.AcademicYear)
            .Include(x => x.Admissions).ThenInclude(x => x.AcademicClass)
            .Include(x => x.Admissions).ThenInclude(x => x.Section)
            .Include(x => x.Guardians)
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.FirstName.Contains(search) ||
                (x.MiddleName != null && x.MiddleName.Contains(search)) ||
                x.LastName.Contains(search) ||
                (x.Mobile != null && x.Mobile.Contains(search)) ||
                (x.Email != null && x.Email.Contains(search)) ||
                x.Admissions.Any(a =>
                    a.TenantId == tenantId &&
                    a.BranchId == branchId &&
                    a.AdmissionNo.Contains(search)));
        }

        if (academicYearId.HasValue)
        {
            query = query.Where(x =>
                x.Admissions.Any(a =>
                    a.TenantId == tenantId &&
                    a.BranchId == branchId &&
                    a.AcademicYearId == academicYearId.Value));
        }

        if (classId.HasValue)
        {
            query = query.Where(x =>
                x.Admissions.Any(a =>
                    a.TenantId == tenantId &&
                    a.BranchId == branchId &&
                    a.AcademicClassId == classId.Value));
        }

        if (sectionId.HasValue)
        {
            query = query.Where(x =>
                x.Admissions.Any(a =>
                    a.TenantId == tenantId &&
                    a.BranchId == branchId &&
                    a.SectionId == sectionId.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

}