using Shala.Application.Repositories.Registration;
using Shala.Domain.Entities.Registration;
using Shala.Domain.Enums;
using Shala.Shared.Common;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;
namespace Shala.Application.Features.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _repo;
        private readonly IRegistrationFeeRepository _feeRepo;

        public RegistrationService(
            IRegistrationRepository repo,
            IRegistrationFeeRepository feeRepo)
        {
            _repo = repo;
            _feeRepo = feeRepo;
        }

        public async Task<int> CreateAsync(
            int tenantId,
            int branchId,
            CreateRegistrationRequest request,
            CancellationToken ct)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            ValidateRequest(
                request.FirstName,
                request.LastName,
                request.GuardianName,
                request.Phone,
                request.Gender,
                request.InterestedClassId);

            var entity = BuildEntity(tenantId, branchId, request);

            var id = await _repo.CreateAsync(entity, ct);

            entity.RegistrationNo = $"REG-{DateTime.UtcNow:yyyy}-{id:D5}";
            await _repo.UpdateAsync(entity, ct);

            return id;
        }

        public Task<PagedResult<RegistrationDto>> GetPagedAsync(
            int tenantId,
            int branchId,
            PagedRequest request,
            CancellationToken ct)
        {
            return _repo.GetPagedAsync(tenantId, branchId, request, ct);
        }

        public Task<List<RegistrationDto>> GetAllAsync(
            int tenantId,
            int branchId,
            CancellationToken ct)
        {
            return _repo.GetAllAsync(tenantId, branchId, ct);
        }

        public async Task<RegistrationDto> GetByIdAsync(
            int tenantId,
            int branchId,
            int id,
            CancellationToken ct)
        {
            var data = await _repo.GetByIdDtoAsync(tenantId, branchId, id, ct);

            if (data is null)
                throw new KeyNotFoundException("Registration not found.");

            return data;
        }

        public async Task UpdateAsync(
            int tenantId,
            int branchId,
            int id,
            UpdateRegistrationRequest request,
            CancellationToken ct)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            ValidateRequest(
                request.FirstName,
                request.LastName,
                request.GuardianName,
                request.Phone,
                request.Gender,
                request.InterestedClassId);

            var entity = await _repo.GetByIdAsync(id, tenantId, branchId, ct);

            if (entity is null || entity.IsDeleted)
                throw new KeyNotFoundException("Registration not found.");

            if (entity.Status == RegistrationStatus.Converted)
                throw new InvalidOperationException("Converted registration cannot be edited.");

            entity.FirstName = request.FirstName.Trim();
            entity.MiddleName = string.IsNullOrWhiteSpace(request.MiddleName) ? null : request.MiddleName.Trim();
            entity.LastName = request.LastName.Trim();
            entity.GuardianName = request.GuardianName.Trim();
            entity.Phone = request.Phone.Trim();
            entity.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
            entity.Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim();
            entity.DateOfBirth = request.DateOfBirth;
            entity.Gender = request.Gender!.Value;
            entity.InterestedClassId = request.InterestedClassId;
            entity.RegistrationDate = request.RegistrationDate;

            await _repo.UpdateAsync(entity, ct);
        }

        public async Task DeleteAsync(
            int tenantId,
            int branchId,
            int id,
            CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(id, tenantId, branchId, ct);

            if (entity is null)
                return;

            if (entity.Status == RegistrationStatus.Converted)
                throw new InvalidOperationException("Converted registration cannot be deleted.");

            if (entity.FeePaid)
                throw new InvalidOperationException("Fee paid registration cannot be deleted.");

            entity.IsDeleted = true;

            await _repo.UpdateAsync(entity, ct);
        }

        public async Task<ConvertRegistrationResponse> ConvertAsync(
            int tenantId,
            int branchId,
            int id,
            ConvertRegistrationRequest request,
            string actor,
            CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(id, tenantId, branchId, ct);

            if (entity is null || entity.IsDeleted)
                throw new KeyNotFoundException("Registration not found.");

            if (!entity.Gender.HasValue || !Enum.IsDefined(typeof(Gender), entity.Gender.Value))
                throw new InvalidOperationException("Gender is required before conversion. Please update the registration first.");

            return await _repo.ConvertToStudentAsync(tenantId, branchId, id, request, actor, ct);
        }

        public Task<RegistrationFeeResponse> SaveWithFeeAsync(
            int tenantId,
            int branchId,
            SaveRegistrationWithFeeRequest request,
            CancellationToken ct)
        {
            return _feeRepo.SaveWithFeeAsync(tenantId, branchId, request, ct);
        }

        private static StudentRegistration BuildEntity(
      int tenantId,
      int branchId,
      CreateRegistrationRequest request)
        {
            return new StudentRegistration
            {
                TenantId = tenantId,
                BranchId = branchId,
                RegistrationDate = request.RegistrationDate,
                FirstName = request.FirstName.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(request.MiddleName) ? null : request.MiddleName.Trim(),
                LastName = request.LastName.Trim(),
                GuardianName = request.GuardianName.Trim(),
                Phone = request.Phone.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
                InterestedClassId = request.InterestedClassId,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender!.Value,

                // 🔥 FIX START
                FeePaid = false,
                PaymentStatus = RegistrationPaymentStatus.Unpaid,
                Status = RegistrationStatus.Pending,
                // 🔥 FIX END

                IsDeleted = false
            };
        }

        private static void ValidateRequest(
            string? firstName,
            string? lastName,
            string? guardianName,
            string? phone,
            Gender? gender,
            int interestedClassId)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new InvalidOperationException("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new InvalidOperationException("Last name is required.");

            if (string.IsNullOrWhiteSpace(guardianName))
                throw new InvalidOperationException("Guardian name is required.");

            if (string.IsNullOrWhiteSpace(phone))
                throw new InvalidOperationException("Phone is required.");

            if (!gender.HasValue)
                throw new InvalidOperationException("Gender is required.");

            if (!Enum.IsDefined(typeof(Gender), gender.Value))
                throw new InvalidOperationException("Invalid gender value.");

            if (interestedClassId <= 0)
                throw new InvalidOperationException("Interested class is required.");
        }
    }
}