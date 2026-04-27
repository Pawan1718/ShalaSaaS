using Microsoft.EntityFrameworkCore;
using Shala.Application.Contracts;
using Shala.Domain.Entities.Academics;
using Shala.Infrastructure.Data;

namespace Shala.Infrastructure.Services;

public class AdmissionNumberGenerator : IAdmissionNumberGenerator
{
    private readonly AppDbContext _context;

    public AdmissionNumberGenerator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var counter = await _context.AdmissionNumberCounters
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicYearId == academicYearId,
                cancellationToken);

        if (counter is null)
        {
            counter = new AdmissionNumberCounter
            {
                TenantId = tenantId,
                BranchId = branchId,
                AcademicYearId = academicYearId,
                LastNumber = 0,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _context.AdmissionNumberCounters.AddAsync(counter, cancellationToken);
        }

        counter.LastNumber += 1;
        counter.UpdatedAtUtc = DateTime.UtcNow;

        return $"ADM-{DateTime.UtcNow.Year}-{counter.LastNumber:D4}";
    }
}