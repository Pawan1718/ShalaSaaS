



using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Application.Repositories.Registration;
using Shala.Domain.Entities.Registration;
using Shala.Domain.Entities.Students;
using Shala.Domain.Enums;
using Shala.Infrastructure.Data;
using Shala.Shared.Common;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Infrastructure.Repositories.Registration
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly AppDbContext _db;

        public RegistrationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(StudentRegistration entity, CancellationToken ct)
        {
            await _db.StudentRegistrations.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task UpdateAsync(StudentRegistration entity, CancellationToken ct)
        {
            _db.StudentRegistrations.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public Task<StudentRegistration?> GetByIdAsync(int id, int tenantId, int branchId, CancellationToken ct)
        {
            return _db.StudentRegistrations
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.TenantId == tenantId &&
                    x.BranchId == branchId &&
                    !x.IsDeleted,
                    ct);
        }

        public Task<RegistrationDto?> GetByIdDtoAsync(int tenantId, int branchId, int id, CancellationToken ct)
        {
            return BuildQuery(tenantId, branchId)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<List<RegistrationDto>> GetAllAsync(int tenantId, int branchId, CancellationToken ct)
        {
            return BuildQuery(tenantId, branchId)
                .OrderByDescending(x => x.Id)
                .ToListAsync(ct);
        }

        public async Task<PagedResult<RegistrationDto>> GetPagedAsync(
            int tenantId,
            int branchId,
            PagedRequest request,
            CancellationToken ct)
        {
            request ??= new PagedRequest();

            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var query = BuildQuery(tenantId, branchId);

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var search = request.SearchText.Trim().ToLower();

                query = query.Where(x =>
                    (x.RegistrationNo ?? string.Empty).ToLower().Contains(search) ||
                    (x.FirstName ?? string.Empty).ToLower().Contains(search) ||
                    (x.MiddleName ?? string.Empty).ToLower().Contains(search) ||
                    (x.LastName ?? string.Empty).ToLower().Contains(search) ||
                    (x.GuardianName ?? string.Empty).ToLower().Contains(search) ||
                    (x.Phone ?? string.Empty).ToLower().Contains(search) ||
                    (x.FullName ?? string.Empty).ToLower().Contains(search));
            }

            query = request.SortDescending
                ? query.OrderByDescending(x => x.Id)
                : query.OrderBy(x => x.Id);

            return await query.ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<ConvertRegistrationResponse> ConvertToStudentAsync(
            int tenantId,
            int branchId,
            int id,
            ConvertRegistrationRequest request,
            string actor,
            CancellationToken ct)
        {
            request ??= new ConvertRegistrationRequest();

            var reg = await GetByIdAsync(id, tenantId, branchId, ct);

            if (reg is null)
                throw new KeyNotFoundException("Registration not found.");

            if (!reg.FeePaid)
                throw new InvalidOperationException("Fee not paid.");

            if (reg.Status == RegistrationStatus.Converted)
                throw new InvalidOperationException("Registration is already converted.");

            if (!request.AcademicYearId.HasValue || request.AcademicYearId.Value <= 0)
                throw new InvalidOperationException("Academic year is required.");

            if (!request.ClassId.HasValue || request.ClassId.Value <= 0)
                throw new InvalidOperationException("Class is required.");

            if (!reg.DateOfBirth.HasValue)
                throw new InvalidOperationException("Date of birth is required before conversion.");

            if (!reg.Gender.HasValue)
                throw new InvalidOperationException("Gender is required before conversion.");

            var academicYear = await _db.AcademicYears
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == request.AcademicYearId.Value &&
                    x.TenantId == tenantId &&
                    x.IsActive,
                    ct);

            if (academicYear is null)
                throw new InvalidOperationException("Academic year not found.");

            var academicClass = await _db.AcademicClasses
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == request.ClassId.Value &&
                    x.TenantId == tenantId &&
                    x.IsActive,
                    ct);

            if (academicClass is null)
                throw new InvalidOperationException("Class not found.");

            await using var transaction = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                var actorName = string.IsNullOrWhiteSpace(actor) ? "System" : actor.Trim();
                var convertedOn = DateTime.UtcNow;

                var student = new Student
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    FirstName = reg.FirstName?.Trim() ?? string.Empty,
                    MiddleName = string.IsNullOrWhiteSpace(reg.MiddleName) ? null : reg.MiddleName.Trim(),
                    LastName = reg.LastName?.Trim() ?? string.Empty,
                    Gender = reg.Gender.Value,
                    DateOfBirth = reg.DateOfBirth.Value,
                    Mobile = string.IsNullOrWhiteSpace(reg.Phone) ? null : reg.Phone.Trim(),
                    Address = string.IsNullOrWhiteSpace(reg.Address) ? null : reg.Address.Trim(),
                    Status = StudentStatus.Active,
                    CreatedAt = convertedOn,
                    CreatedBy = actorName,
                    Guardians = new List<Guardian>
                    {
                        new Guardian
                        {
                            TenantId = tenantId,
                            Name = reg.GuardianName?.Trim() ?? string.Empty,
                            RelationType = RelationType.Guardian,
                            Mobile = string.IsNullOrWhiteSpace(reg.Phone) ? null : reg.Phone.Trim(),
                            Address = string.IsNullOrWhiteSpace(reg.Address) ? null : reg.Address.Trim(),
                            IsPrimary = true,
                            CreatedAt = convertedOn,
                            CreatedBy = actorName
                        }
                    }
                };

                await _db.Students.AddAsync(student, ct);
                await _db.SaveChangesAsync(ct);

                var admissionCount = await _db.StudentAdmissions.CountAsync(x =>
                    x.TenantId == tenantId &&
                    x.BranchId == branchId &&
                    x.AcademicYearId == request.AcademicYearId.Value,
                    ct);

                var admission = new StudentAdmission
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    StudentId = student.Id,
                    AcademicYearId = request.AcademicYearId.Value,
                    AcademicClassId = request.ClassId.Value,
                    SectionId = null,
                    AdmissionNo = $"ADM-{DateTime.UtcNow.Year}-{(admissionCount + 1):D4}",
                    RollNo = null,
                    AdmissionDate = request.AdmissionDate?.Date ?? DateTime.Today,
                    Status = AdmissionStatus.Active,
                    IsCurrent = true,
                    CreatedAt = convertedOn,
                    CreatedBy = actorName
                };

                await _db.StudentAdmissions.AddAsync(admission, ct);
                await _db.SaveChangesAsync(ct);

                reg.StudentId = student.Id;
                reg.StudentAdmissionId = admission.Id;
                reg.Status = RegistrationStatus.Converted;
                reg.ConvertedOn = convertedOn;
                reg.ConvertedBy = actorName;

                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return new ConvertRegistrationResponse
                {
                    RegistrationId = reg.Id,
                    RegistrationNo = reg.RegistrationNo ?? string.Empty,
                    StudentId = student.Id,
                    StudentAdmissionId = admission.Id,
                    AdmissionNo = admission.AdmissionNo ?? string.Empty,
                    ConvertedOn = convertedOn
                };
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        private IQueryable<RegistrationDto> BuildQuery(int tenantId, int branchId)
        {
            var registrations = _db.StudentRegistrations
                .AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.BranchId == branchId &&
                    !x.IsDeleted);

            var classes = _db.AcademicClasses
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.IsActive);

            var receipts = _db.RegistrationFeeReceipts
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.BranchId == branchId);

            return
                from reg in registrations
                join cls in classes on reg.InterestedClassId equals cls.Id into classGroup
                from cls in classGroup.DefaultIfEmpty()
                select new RegistrationDto
                {
                    Id = reg.Id,
                    RegistrationNo = reg.RegistrationNo ?? string.Empty,
                    RegistrationDate = reg.RegistrationDate,
                    FirstName = reg.FirstName ?? string.Empty,
                    MiddleName = reg.MiddleName,
                    LastName = reg.LastName ?? string.Empty,
                    DateOfBirth = reg.DateOfBirth,
                    Gender = reg.Gender.HasValue ? reg.Gender.Value.ToString() : null,
                    GuardianName = reg.GuardianName ?? string.Empty,
                    Phone = reg.Phone ?? string.Empty,
                    Address = reg.Address ?? string.Empty,
                    Note = reg.Note,
                    InterestedClassId = reg.InterestedClassId,
                    InterestedClassName = cls != null ? cls.Name : string.Empty,
                    FeePaid = reg.FeePaid,
                    Status = reg.Status.ToString(),
                    StudentId = reg.StudentId,
                    StudentAdmissionId = reg.StudentAdmissionId,
                    ConvertedOn = reg.ConvertedOn,
                    LatestReceiptId = receipts
                        .Where(r => r.RegistrationId == reg.Id)
                        .OrderByDescending(r => r.Id)
                        .Select(r => (int?)r.Id)
                        .FirstOrDefault(),
                    LatestReceiptNo = receipts
                        .Where(r => r.RegistrationId == reg.Id)
                        .OrderByDescending(r => r.Id)
                        .Select(r => r.ReceiptNo)
                        .FirstOrDefault()
                };
        }
    }
}