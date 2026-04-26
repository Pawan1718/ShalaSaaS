using Shala.Application.Common;
using Shala.Application.Repositories.Students;
using Shala.Domain.Entities.Students;
using Shala.Domain.Enums;
using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Students;

public class StudentGuardianService : IStudentGuardianService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentGuardianRepository _studentGuardianRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentGuardianService(
        IStudentRepository studentRepository,
        IStudentGuardianRepository studentGuardianRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _studentGuardianRepository = studentGuardianRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GuardianResponse>> AddAsync(
        int tenantId,
        int branchId,
        string actor,
        int studentId,
        CreateGuardianRequest request,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(studentId, tenantId, branchId, cancellationToken);
        if (student is null)
            return ApiResponse<GuardianResponse>.Fail("Student not found.");

        var guardianCount = await _studentGuardianRepository.GetGuardianCountAsync(studentId, tenantId, cancellationToken);
        if (guardianCount >= 4)
            return ApiResponse<GuardianResponse>.Fail("Maximum 4 guardians allowed.");

        var studentDetails = await _studentRepository.GetDetailsAsync(studentId, tenantId, branchId, cancellationToken);
        if (studentDetails is null)
            return ApiResponse<GuardianResponse>.Fail("Student not found.");

        if (request.IsPrimary)
        {
            foreach (var g in studentDetails.Guardians)
                g.IsPrimary = false;
        }

        var guardian = new Guardian
        {
            TenantId = tenantId,
            StudentId = studentId,
            Name = request.Name.Trim(),
            RelationType = (RelationType)request.RelationType,
            Mobile = request.Mobile.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Occupation = string.IsNullOrWhiteSpace(request.Occupation) ? null : request.Occupation.Trim(),
            Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
            IsPrimary = request.IsPrimary,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = actor
        };

        await _studentGuardianRepository.AddGuardianAsync(guardian, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<GuardianResponse>.Ok(new GuardianResponse
        {
            Id = guardian.Id,
            Name = guardian.Name,
            RelationType = guardian.RelationType.ToString(),
            Mobile = guardian.Mobile,
            Email = guardian.Email,
            IsPrimary = guardian.IsPrimary
        }, "Guardian added successfully.");
    }

    public async Task<ApiResponse<GuardianResponse>> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        int studentId,
        int guardianId,
        CreateGuardianRequest request,
        CancellationToken cancellationToken = default)
    {
        var studentDetails = await _studentRepository.GetDetailsAsync(studentId, tenantId, branchId, cancellationToken);
        if (studentDetails is null)
            return ApiResponse<GuardianResponse>.Fail("Student not found.");

        var guardian = await _studentGuardianRepository.GetGuardianByIdAsync(guardianId, studentId, tenantId, cancellationToken);
        if (guardian is null)
            return ApiResponse<GuardianResponse>.Fail("Guardian not found.");

        if (request.IsPrimary)
        {
            foreach (var item in studentDetails.Guardians)
                item.IsPrimary = false;
        }

        guardian.Name = request.Name.Trim();
        guardian.RelationType = (RelationType)request.RelationType;
        guardian.Mobile = request.Mobile.Trim();
        guardian.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        guardian.Occupation = string.IsNullOrWhiteSpace(request.Occupation) ? null : request.Occupation.Trim();
        guardian.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        guardian.IsPrimary = request.IsPrimary;
        guardian.UpdatedAt = DateTime.UtcNow;
        guardian.UpdatedBy = actor;

        _studentGuardianRepository.UpdateGuardian(guardian);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<GuardianResponse>.Ok(new GuardianResponse
        {
            Id = guardian.Id,
            Name = guardian.Name,
            RelationType = guardian.RelationType.ToString(),
            Mobile = guardian.Mobile,
            Email = guardian.Email,
            IsPrimary = guardian.IsPrimary
        }, "Guardian updated successfully.");
    }

    public async Task<ApiResponse<bool>> RemoveAsync(
        int tenantId,
        int branchId,
        int studentId,
        int guardianId,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(studentId, tenantId, branchId, cancellationToken);
        if (student is null)
            return ApiResponse<bool>.Fail("Student not found.");

        var guardian = await _studentGuardianRepository.GetGuardianByIdAsync(guardianId, studentId, tenantId, cancellationToken);
        if (guardian is null)
            return ApiResponse<bool>.Fail("Guardian not found.");

        _studentGuardianRepository.DeleteGuardian(guardian);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Guardian removed successfully.");
    }
}