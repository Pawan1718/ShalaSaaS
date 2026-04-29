using Microsoft.EntityFrameworkCore;
using Shala.Application.Repositories.Students;
using Shala.Domain.Entities.Students;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Repositories.Students;

public class StudentGuardianRepository : IStudentGuardianRepository
{
    private readonly AppDbContext _context;

    public StudentGuardianRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guardian?> GetGuardianByIdAsync(
        int guardianId,
        int studentId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Guardian>()
            .FirstOrDefaultAsync(x =>
                x.Id == guardianId &&
                x.StudentId == studentId &&
                x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<int> GetGuardianCountAsync(
        int studentId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Guardian>()
            .CountAsync(x => x.StudentId == studentId && x.TenantId == tenantId, cancellationToken);
    }

    public async Task AddGuardianAsync(Guardian guardian, CancellationToken cancellationToken = default)
    {
        await _context.Set<Guardian>().AddAsync(guardian, cancellationToken);
    }

    public void UpdateGuardian(Guardian guardian)
    {
        _context.Set<Guardian>().Update(guardian);
    }

    public void DeleteGuardian(Guardian guardian)
    {
        _context.Set<Guardian>().Remove(guardian);
    }
}