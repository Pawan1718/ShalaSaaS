//using Microsoft.EntityFrameworkCore;
//using Shala.Application.Contracts;
//using Shala.Domain.Entities.Academics;
//using Shala.Infrastructure.Data;

//namespace Shala.Infrastructure.Services;

//public class AdmissionNumberGenerator : IAdmissionNumberGenerator
//{
//    private readonly AppDbContext _context;

//    public AdmissionNumberGenerator(AppDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<string> GenerateAsync(
//        int tenantId,
//        int branchId,
//        int academicYearId,
//        CancellationToken cancellationToken = default)
//    {
//        var year = DateTime.UtcNow.Year;

//        // SERIALIZABLE already applied from service layer
//        var counter = await _context.AdmissionNumberCounters
//            .FirstOrDefaultAsync(x =>
//                x.TenantId == tenantId &&
//                x.BranchId == branchId &&
//                x.AcademicYearId == academicYearId,
//                cancellationToken);

//        if (counter is null)
//        {
//            counter = new AdmissionNumberCounter
//            {
//                TenantId = tenantId,
//                BranchId = branchId,
//                AcademicYearId = academicYearId,
//                LastNumber = 1,
//                UpdatedAtUtc = DateTime.UtcNow
//            };

//            _context.AdmissionNumberCounters.Add(counter);
//        }
//        else
//        {
//            counter.LastNumber += 1;
//            counter.UpdatedAtUtc = DateTime.UtcNow;
//        }

//        await _context.SaveChangesAsync(cancellationToken);

//        return $"ADM-{year}-{counter.LastNumber:D4}";
//    }
//}




using System.Text.RegularExpressions;
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
        var year = DateTime.UtcNow.Year;

        var counter = await _context.AdmissionNumberCounters
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicYearId == academicYearId,
                cancellationToken);

        if (counter is null)
        {
            var lastExistingNumber = await GetLastExistingAdmissionNumberAsync(
                tenantId,
                branchId,
                academicYearId,
                year,
                cancellationToken);

            counter = new AdmissionNumberCounter
            {
                TenantId = tenantId,
                BranchId = branchId,
                AcademicYearId = academicYearId,
                LastNumber = lastExistingNumber + 1,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _context.AdmissionNumberCounters.Add(counter);
        }
        else
        {
            counter.LastNumber += 1;
            counter.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return $"ADM-{year}-{counter.LastNumber:D4}";
    }

    private async Task<int> GetLastExistingAdmissionNumberAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int year,
        CancellationToken cancellationToken)
    {
        var admissionNos = await _context.StudentAdmissions
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.BranchId == branchId &&
                x.AcademicYearId == academicYearId &&
                x.AdmissionNo.StartsWith($"ADM-{year}-"))
            .Select(x => x.AdmissionNo)
            .ToListAsync(cancellationToken);

        var max = 0;

        foreach (var admissionNo in admissionNos)
        {
            var match = Regex.Match(admissionNo, @$"^ADM-{year}-(\d+)$");

            if (!match.Success)
                continue;

            if (int.TryParse(match.Groups[1].Value, out var number))
                max = Math.Max(max, number);
        }

        return max;
    }
}