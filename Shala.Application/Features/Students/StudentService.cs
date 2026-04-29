using Shala.Application.Common;
using Shala.Application.Repositories.Students;
using Shala.Domain.Entities.Students;
using Shala.Domain.Enums;
using Shala.Shared.Common;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Students;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<StudentDetailsResponse>> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        CreateStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        var guardians = request.Guardians ?? new List<CreateGuardianRequest>();

        var student = new Student
        {
            TenantId = tenantId,
            BranchId = branchId,
            FirstName = request.FirstName.Trim(),
            MiddleName = string.IsNullOrWhiteSpace(request.MiddleName) ? null : request.MiddleName.Trim(),
            LastName = request.LastName.Trim(),
            Gender = (Gender)request.Gender,
            DateOfBirth = request.DateOfBirth,
            AadhaarNo = string.IsNullOrWhiteSpace(request.AadhaarNo) ? null : request.AadhaarNo.Trim(),
            BloodGroup = string.IsNullOrWhiteSpace(request.BloodGroup) ? null : request.BloodGroup.Trim(),
            Mobile = string.IsNullOrWhiteSpace(request.Mobile) ? null : request.Mobile.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
            PhotoUrl = string.IsNullOrWhiteSpace(request.PhotoUrl) ? null : request.PhotoUrl.Trim(),
            Status = StudentStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = actor,
            Guardians = guardians.Select(g => new Guardian
            {
                TenantId = tenantId,
                Name = g.Name.Trim(),
                RelationType = (RelationType)g.RelationType,
                Mobile = g.Mobile.Trim(),
                Email = string.IsNullOrWhiteSpace(g.Email) ? null : g.Email.Trim(),
                Occupation = string.IsNullOrWhiteSpace(g.Occupation) ? null : g.Occupation.Trim(),
                Address = string.IsNullOrWhiteSpace(g.Address) ? null : g.Address.Trim(),
                IsPrimary = g.IsPrimary,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = actor
            }).ToList()
        };

        await _studentRepository.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var savedStudent = await _studentRepository.GetDetailsAsync(student.Id, tenantId, branchId, cancellationToken);

        if (savedStudent is null)
            return ApiResponse<StudentDetailsResponse>.Fail("Student created but could not be loaded.");

        return ApiResponse<StudentDetailsResponse>.Ok(
            StudentMapper.MapStudentDetails(savedStudent),
            "Student created");
    }

    public async Task<ApiResponse<StudentDetailsResponse>> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        int id,
        UpdateStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetDetailsAsync(id, tenantId, branchId, cancellationToken);

        if (student is null)
            return ApiResponse<StudentDetailsResponse>.Fail("Student not found");

        student.FirstName = request.FirstName.Trim();
        student.MiddleName = string.IsNullOrWhiteSpace(request.MiddleName) ? null : request.MiddleName.Trim();
        student.LastName = request.LastName.Trim();
        student.Gender = (Gender)request.Gender;
        student.DateOfBirth = request.DateOfBirth;
        student.AadhaarNo = string.IsNullOrWhiteSpace(request.AadhaarNo) ? null : request.AadhaarNo.Trim();
        student.BloodGroup = string.IsNullOrWhiteSpace(request.BloodGroup) ? null : request.BloodGroup.Trim();
        student.Mobile = string.IsNullOrWhiteSpace(request.Mobile) ? null : request.Mobile.Trim();
        student.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        student.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        student.PhotoUrl = string.IsNullOrWhiteSpace(request.PhotoUrl) ? null : request.PhotoUrl.Trim();
        student.Status = (StudentStatus)request.Status;
        student.UpdatedAt = DateTime.UtcNow;
        student.UpdatedBy = actor;

        _studentRepository.Update(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<StudentDetailsResponse>.Ok(
            StudentMapper.MapStudentDetails(student),
            "Student updated");
    }

    public async Task<ApiResponse<StudentDetailsResponse>> GetByIdAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetDetailsAsync(id, tenantId, branchId, cancellationToken);

        if (student is null)
            return ApiResponse<StudentDetailsResponse>.Fail("Student not found");

        return ApiResponse<StudentDetailsResponse>.Ok(StudentMapper.MapStudentDetails(student));
    }

    public async Task<ApiResponse<PagedResult<StudentListItemResponse>>> GetPagedAsync(
        int tenantId,
        int branchId,
        StudentListRequest request,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _studentRepository.GetPagedAsync(
            tenantId,
            branchId,
            request.Search,
            request.AcademicYearId > 0 ? request.AcademicYearId : null,
            request.ClassId > 0 ? request.ClassId : null,
            request.SectionId > 0 ? request.SectionId : null,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var paged = new PagedResult<StudentListItemResponse>
        {
            Items = items.Select(StudentMapper.MapStudentListItem).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<StudentListItemResponse>>.Ok(paged);
    }
}