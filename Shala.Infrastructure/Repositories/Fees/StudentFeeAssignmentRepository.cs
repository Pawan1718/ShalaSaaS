using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Fees;

public class StudentFeeAssignmentRepository : GenericRepository<StudentFeeAssignment>, IStudentFeeAssignmentRepository
{
    public StudentFeeAssignmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<StudentFeeAssignment>> GetAllAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.Student)
            .Include(x => x.StudentAdmission)
            .Include(x => x.FeeStructure)
                .ThenInclude(x => x.Items)
            .Include(x => x.Charges)
            .Where(x => x.TenantId == tenantId && x.BranchId == branchId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentFeeAssignment?> GetByIdAsync(
        int id,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.Student)
            .Include(x => x.StudentAdmission)
            .Include(x => x.FeeStructure)
                .ThenInclude(x => x.Items)
            .Include(x => x.Charges)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<StudentFeeAssignment?> GetByAdmissionIdAsync(
        int studentAdmissionId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        return await _table
            .Include(x => x.Student)
            .Include(x => x.StudentAdmission)
            .Include(x => x.FeeStructure)
                .ThenInclude(x => x.Items)
            .Include(x => x.Charges)
            .FirstOrDefaultAsync(
                x => x.StudentAdmissionId == studentAdmissionId &&
                     x.TenantId == tenantId &&
                     x.BranchId == branchId,
                cancellationToken);
    }

    public async Task<bool> IsFirstAdmissionForStudentAsync(
        int studentId,
        int studentAdmissionId,
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        var firstAdmissionId = await _db.StudentAdmissions
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                x.TenantId == tenantId &&
                x.BranchId == branchId)
            .OrderBy(x => x.AdmissionDate)
            .ThenBy(x => x.Id)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return firstAdmissionId.HasValue && firstAdmissionId.Value == studentAdmissionId;
    }
}