using Shala.Application.Common;
using Shala.Application.Features.TenantConfig;
using Shala.Application.Repositories.Students;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Academics;
using Shala.Domain.Entities.Students;
using Shala.Domain.Enums;
using Shala.Shared.Common;
using Shala.Shared.Enums;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Students;

public class StudentAdmissionService : IStudentAdmissionService
{
    private readonly IStudentAdmissionRepository _studentAdmissionRepository;
    private readonly IRollNumberGeneratorService _rollNumberGeneratorService;
    private readonly IRollNumberSettingRepository _rollNumberSettingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentAdmissionService(
     IStudentAdmissionRepository studentAdmissionRepository,
     IRollNumberGeneratorService rollNumberGeneratorService,
     IRollNumberSettingRepository rollNumberSettingRepository,
     IUnitOfWork unitOfWork)
    {
        _studentAdmissionRepository = studentAdmissionRepository;
        _rollNumberGeneratorService = rollNumberGeneratorService;
        _rollNumberSettingRepository = rollNumberSettingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<StudentAdmissionResponse>> CreateAsync(
        int tenantId,
        int branchId,
        string actor,
        CreateStudentAdmissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var studentExists = await _studentAdmissionRepository.StudentExistsAsync(
            request.StudentId,
            tenantId,
            branchId,
            cancellationToken);

        if (!studentExists)
            return ApiResponse<StudentAdmissionResponse>.Fail("Student not found.");

        var academicYear = await _studentAdmissionRepository.GetAcademicYearAsync(
            request.AcademicYearId,
            tenantId,
            cancellationToken);

        if (academicYear is null)
            return ApiResponse<StudentAdmissionResponse>.Fail("Academic year not found.");

        var academicClass = await _studentAdmissionRepository.GetAcademicClassAsync(
            request.ClassId,
            tenantId,
            cancellationToken);

        if (academicClass is null)
            return ApiResponse<StudentAdmissionResponse>.Fail("Class not found.");

        Section? section = null;

        if (request.SectionId.HasValue)
        {
            section = await _studentAdmissionRepository.GetSectionAsync(
                request.SectionId.Value,
                tenantId,
                branchId,
                request.ClassId,
                cancellationToken);

            if (section is null)
                return ApiResponse<StudentAdmissionResponse>.Fail("Section not found for selected class.");
        }
        if (section is not null && section.Capacity.HasValue)
        {
            var currentCount = await _studentAdmissionRepository.GetSectionAdmissionCountAsync(
                tenantId,
                branchId,
                request.AcademicYearId,
                request.ClassId,
                section.Id,
                cancellationToken);

            if (currentCount >= section.Capacity.Value)
                return ApiResponse<StudentAdmissionResponse>.Fail("Selected section is already full.");
        }
        var duplicateAdmission = await _studentAdmissionRepository.AdmissionExistsAsync(
            request.StudentId,
            tenantId,
            request.AcademicYearId,
            cancellationToken);

        if (duplicateAdmission)
            return ApiResponse<StudentAdmissionResponse>.Fail("Admission already exists for this academic year.");

        string? finalRollNo;

        try
        {
            finalRollNo = await _rollNumberGeneratorService.ResolveRollNoForCreateAsync(
                tenantId,
                branchId,
                request.AcademicYearId,
                request.ClassId,
                request.SectionId,
                request.RollNo,
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<StudentAdmissionResponse>.Fail(ex.Message);
        }

        var runningCount = await _studentAdmissionRepository.GetAdmissionCountAsync(
            tenantId,
            branchId,
            request.AcademicYearId,
            cancellationToken);

        var admission = new StudentAdmission
        {
            StudentId = request.StudentId,
            TenantId = tenantId,
            BranchId = branchId,
            AcademicYearId = request.AcademicYearId,
            AcademicClassId = request.ClassId,
            SectionId = request.SectionId,
            AdmissionNo = $"ADM-{DateTime.UtcNow.Year}-{(runningCount + 1):D4}",
            AdmissionDate = request.AdmissionDate,
            RollNo = finalRollNo,
            Status = AdmissionStatus.Active,
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = actor
        };

        await _studentAdmissionRepository.AddAdmissionAsync(admission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<StudentAdmissionResponse>.Ok(new StudentAdmissionResponse
        {
            Id = admission.Id,
            StudentId = admission.StudentId,
            AdmissionNo = admission.AdmissionNo,
            AcademicYear = academicYear.Name,
            ClassName = academicClass.Name,
            SectionName = section?.Name,
            RollNo = admission.RollNo,
            AdmissionDate = admission.AdmissionDate,
            Status = admission.Status.ToString()
        }, "Admission created successfully.");
    }

    public async Task<ApiResponse<StudentAdmissionResponse>> UpdateAsync(
        int tenantId,
        int branchId,
        string actor,
        int id,
        UpdateStudentAdmissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var admission = await _studentAdmissionRepository.GetAdmissionByIdAsync(
            id,
            tenantId,
            branchId,
            cancellationToken);

        if (admission is null)
            return ApiResponse<StudentAdmissionResponse>.Fail("Admission not found.");

        var academicYear = await _studentAdmissionRepository.GetAcademicYearAsync(
            request.AcademicYearId,
            tenantId,
            cancellationToken);

        if (academicYear is null)
            return ApiResponse<StudentAdmissionResponse>.Fail("Academic year not found.");

        var academicClass = await _studentAdmissionRepository.GetAcademicClassAsync(
            request.ClassId,
            tenantId,
            cancellationToken);

        if (academicClass is null)
            return ApiResponse<StudentAdmissionResponse>.Fail("Class not found.");

        Section? section = null;

        if (request.SectionId.HasValue)
        {
            section = await _studentAdmissionRepository.GetSectionAsync(
                request.SectionId.Value,
                tenantId,
                branchId,
                request.ClassId,
                cancellationToken);

            if (section is null)
                return ApiResponse<StudentAdmissionResponse>.Fail("Section not found for selected class.");
        }
        if (section is not null && section.Capacity.HasValue)
        {
            var currentCount = await _studentAdmissionRepository.GetSectionAdmissionCountAsync(
                tenantId,
                branchId,
                request.AcademicYearId,
                request.ClassId,
                section.Id,
                cancellationToken);

            var movingWithinSameScope =
                admission.AcademicYearId == request.AcademicYearId &&
                admission.AcademicClassId == request.ClassId &&
                admission.SectionId == request.SectionId;

            if (!movingWithinSameScope && currentCount >= section.Capacity.Value)
                return ApiResponse<StudentAdmissionResponse>.Fail("Selected section is already full.");
        }

        var duplicateAdmission = await _studentAdmissionRepository.AdmissionExistsAsync(
            request.StudentId,
            tenantId,
            request.AcademicYearId,
            cancellationToken);

        if (duplicateAdmission && admission.AcademicYearId != request.AcademicYearId)
            return ApiResponse<StudentAdmissionResponse>.Fail("Admission already exists for this academic year.");

        string? finalRollNo;

        try
        {
            finalRollNo = await _rollNumberGeneratorService.ResolveRollNoForUpdateAsync(
                tenantId,
                branchId,
                request.AcademicYearId,
                request.ClassId,
                request.SectionId,
                request.Id,
                admission.AcademicYearId,
                admission.AcademicClassId,
                admission.SectionId,
                request.RollNo,
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<StudentAdmissionResponse>.Fail(ex.Message);
        }

        admission.AcademicYearId = request.AcademicYearId;
        admission.AcademicClassId = request.ClassId;
        admission.SectionId = request.SectionId;
        admission.AdmissionDate = request.AdmissionDate;
        admission.RollNo = finalRollNo;
        admission.UpdatedAt = DateTime.UtcNow;
        admission.UpdatedBy = actor;

        _studentAdmissionRepository.UpdateAdmission(admission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<StudentAdmissionResponse>.Ok(new StudentAdmissionResponse
        {
            Id = admission.Id,
            StudentId = admission.StudentId,
            AdmissionNo = admission.AdmissionNo,
            AcademicYear = academicYear.Name,
            ClassName = academicClass.Name,
            SectionName = section?.Name,
            RollNo = admission.RollNo,
            AdmissionDate = admission.AdmissionDate,
            Status = admission.Status.ToString()
        }, "Admission updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(
        int tenantId,
        int branchId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var admission = await _studentAdmissionRepository.GetAdmissionByIdAsync(
            id,
            tenantId,
            branchId,
            cancellationToken);

        if (admission is null)
            return ApiResponse<bool>.Fail("Admission not found.");

        _studentAdmissionRepository.DeleteAdmission(admission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Admission deleted successfully.");
    }

    public async Task<ApiResponse<SectionRollAssignmentDetailResponse>> GetAssignmentDetailAsync(
    int tenantId,
    int branchId,
    int admissionId,
    CancellationToken cancellationToken = default)
    {
        var admission = await _studentAdmissionRepository.GetAdmissionByIdAsync(
            admissionId,
            tenantId,
            branchId,
            cancellationToken);

        if (admission is null)
            return ApiResponse<SectionRollAssignmentDetailResponse>.Fail("Admission not found.");

        var setting = await _rollNumberSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        var response = new SectionRollAssignmentDetailResponse
        {
            StudentAdmissionId = admission.Id,
            StudentId = admission.StudentId,
            StudentName = BuildStudentName(admission.Student),
            AdmissionNo = admission.AdmissionNo,
            AcademicYearId = admission.AcademicYearId,
            AcademicYearName = admission.AcademicYear?.Name ?? string.Empty,
            ClassId = admission.AcademicClassId,
            ClassName = admission.AcademicClass?.Name ?? string.Empty,
            CurrentSectionId = admission.SectionId,
            CurrentSectionName = admission.Section?.Name,
            CurrentRollNo = admission.RollNo,
            AdmissionDate = admission.AdmissionDate,
            Status = admission.Status.ToString(),
            AutoGenerateEnabled = setting?.AutoGenerate ?? false,
            AllowManualOverride = setting?.AllowManualOverride ?? true
        };

        return ApiResponse<SectionRollAssignmentDetailResponse>.Ok(response);
    }

    public async Task<ApiResponse<SectionRollAssignmentPreviewResponse>> GetAssignmentPreviewAsync(
        int tenantId,
        int branchId,
        SectionRollAssignmentPreviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var admission = await _studentAdmissionRepository.GetAdmissionByIdAsync(
            request.StudentAdmissionId,
            tenantId,
            branchId,
            cancellationToken);

        if (admission is null)
            return ApiResponse<SectionRollAssignmentPreviewResponse>.Fail("Admission not found.");

        Section? section = null;
        var currentStrength = 0;
        int? capacity = null;
        var availableSeats = 0;
        var message = string.Empty;

        if (request.SectionId.HasValue)
        {
            section = await _studentAdmissionRepository.GetSectionAsync(
                request.SectionId.Value,
                tenantId,
                branchId,
                admission.AcademicClassId,
                cancellationToken);

            if (section is null)
                return ApiResponse<SectionRollAssignmentPreviewResponse>.Fail("Section not found.");

            currentStrength = await _studentAdmissionRepository.GetSectionAdmissionCountAsync(
                tenantId,
                branchId,
                admission.AcademicYearId,
                admission.AcademicClassId,
                section.Id,
                cancellationToken);

            var sameScope =
                admission.AcademicYearId == admission.AcademicYearId &&
                admission.AcademicClassId == admission.AcademicClassId &&
                admission.SectionId == section.Id;

            if (sameScope && currentStrength > 0)
                currentStrength--;

            capacity = section.Capacity;
            availableSeats = capacity.HasValue
                ? Math.Max(0, capacity.Value - currentStrength)
                : 0;
        }

        var normalizedRollNo = string.IsNullOrWhiteSpace(request.RollNo) ? null : request.RollNo.Trim();
        var rollNoAlreadyExists = false;

        if (!string.IsNullOrWhiteSpace(normalizedRollNo))
        {
            rollNoAlreadyExists = await _studentAdmissionRepository.RollNoExistsAsync(
                tenantId,
                branchId,
                admission.AcademicYearId,
                admission.AcademicClassId,
                request.SectionId,
                normalizedRollNo,
                admission.Id,
                cancellationToken);

            if (rollNoAlreadyExists)
                message = "Roll number already exists in the selected section.";
        }

        string? nextSuggestedRollNo = null;

        if (request.AutoGenerateRollNo)
        {
            try
            {
                nextSuggestedRollNo = await _rollNumberGeneratorService.ResolveRollNoForUpdateAsync(
                    tenantId,
                    branchId,
                    admission.AcademicYearId,
                    admission.AcademicClassId,
                    request.SectionId,
                    admission.Id,
                    admission.AcademicYearId,
                    admission.AcademicClassId,
                    admission.SectionId,
                    null,
                    cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                message = ex.Message;
            }
        }

        var response = new SectionRollAssignmentPreviewResponse
        {
            SectionId = request.SectionId,
            SectionName = section?.Name,
            CurrentStrength = currentStrength,
            Capacity = capacity,
            AvailableSeats = availableSeats,
            NextSuggestedRollNo = nextSuggestedRollNo,
            RollNoAlreadyExists = rollNoAlreadyExists,
            Message = message
        };

        return ApiResponse<SectionRollAssignmentPreviewResponse>.Ok(response);
    }

    public async Task<ApiResponse<StudentAdmissionResponse>> AssignSectionAndRollNoAsync(
        int tenantId,
        int branchId,
        string actor,
        AssignSectionRollRequest request,
        CancellationToken cancellationToken = default)
    {
        var admission = await _studentAdmissionRepository.GetAdmissionByIdAsync(
            request.StudentAdmissionId,
            tenantId,
            branchId,
            cancellationToken);

        if (admission is null)
            return ApiResponse<StudentAdmissionResponse>.Fail("Admission not found.");

        Section? section = null;

        if (request.SectionId.HasValue)
        {
            section = await _studentAdmissionRepository.GetSectionAsync(
                request.SectionId.Value,
                tenantId,
                branchId,
                request.ClassId,
                cancellationToken);

            if (section is null)
                return ApiResponse<StudentAdmissionResponse>.Fail("Section not found for selected class.");

            if (section.Capacity.HasValue)
            {
                var currentCount = await _studentAdmissionRepository.GetSectionAdmissionCountAsync(
                    tenantId,
                    branchId,
                    request.AcademicYearId,
                    request.ClassId,
                    section.Id,
                    cancellationToken);

                var sameScope =
                    admission.AcademicYearId == request.AcademicYearId &&
                    admission.AcademicClassId == request.ClassId &&
                    admission.SectionId == request.SectionId;

                if (!sameScope && currentCount >= section.Capacity.Value)
                    return ApiResponse<StudentAdmissionResponse>.Fail("Selected section is already full.");
            }
        }

        string? finalRollNo;

        try
        {
            if (request.AutoGenerateRollNo)
            {
                finalRollNo = await _rollNumberGeneratorService.ResolveRollNoForUpdateAsync(
                    tenantId,
                    branchId,
                    request.AcademicYearId,
                    request.ClassId,
                    request.SectionId,
                    admission.Id,
                    admission.AcademicYearId,
                    admission.AcademicClassId,
                    admission.SectionId,
                    null,
                    cancellationToken);
            }
            else
            {
                finalRollNo = string.IsNullOrWhiteSpace(request.RollNo) ? null : request.RollNo.Trim();

                if (string.IsNullOrWhiteSpace(finalRollNo))
                    return ApiResponse<StudentAdmissionResponse>.Fail("Roll number is required in manual mode.");

                var exists = await _studentAdmissionRepository.RollNoExistsAsync(
                    tenantId,
                    branchId,
                    request.AcademicYearId,
                    request.ClassId,
                    request.SectionId,
                    finalRollNo,
                    admission.Id,
                    cancellationToken);

                if (exists)
                    return ApiResponse<StudentAdmissionResponse>.Fail("Roll number already exists in the selected section.");
            }
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<StudentAdmissionResponse>.Fail(ex.Message);
        }

        var academicYear = await _studentAdmissionRepository.GetAcademicYearAsync(
            request.AcademicYearId,
            tenantId,
            cancellationToken);

        if (academicYear is null)
            return ApiResponse<StudentAdmissionResponse>.Fail("Academic year not found.");

        var academicClass = await _studentAdmissionRepository.GetAcademicClassAsync(
            request.ClassId,
            tenantId,
            cancellationToken);

        if (academicClass is null)
            return ApiResponse<StudentAdmissionResponse>.Fail("Class not found.");

        admission.AcademicYearId = request.AcademicYearId;
        admission.AcademicClassId = request.ClassId;
        admission.SectionId = request.SectionId;
        admission.RollNo = finalRollNo;
        admission.UpdatedAt = DateTime.UtcNow;
        admission.UpdatedBy = actor;

        _studentAdmissionRepository.UpdateAdmission(admission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new StudentAdmissionResponse
        {
            Id = admission.Id,
            StudentId = admission.StudentId,
            AdmissionNo = admission.AdmissionNo,
            AcademicYear = academicYear.Name,
            ClassName = academicClass.Name,
            SectionName = section?.Name ?? string.Empty,
            RollNo = admission.RollNo,
            AdmissionDate = admission.AdmissionDate,
            Status = admission.Status.ToString()
        };

        return ApiResponse<StudentAdmissionResponse>.Ok(response, "Section and roll number assigned successfully.");
    }


    public async Task<ApiResponse<BulkSectionRollAssignmentPreviewResponse>> GetBulkAssignmentPreviewAsync(
    int tenantId,
    int branchId,
    BulkSectionRollAssignmentRequest request,
    CancellationToken cancellationToken = default)
    {
        try
        {
            var preview = await BuildBulkAssignmentPreviewAsync(
                tenantId,
                branchId,
                request,
                cancellationToken);

            return ApiResponse<BulkSectionRollAssignmentPreviewResponse>.Ok(preview);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<BulkSectionRollAssignmentPreviewResponse>.Fail(ex.Message);
        }
    }

    public async Task<ApiResponse<List<StudentAdmissionResponse>>> ApplyBulkAssignmentAsync(
        int tenantId,
        int branchId,
        string actor,
        BulkSectionRollAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var preview = await BuildBulkAssignmentPreviewAsync(
                tenantId,
                branchId,
                request,
                cancellationToken);

            if (preview.Items.Count == 0)
                return ApiResponse<List<StudentAdmissionResponse>>.Fail("No students selected.");

            var conflicting = preview.Items.Where(x => x.HasConflict).ToList();
            if (conflicting.Count > 0)
                return ApiResponse<List<StudentAdmissionResponse>>.Fail("Preview contains conflicts. Please fix them before applying.");

            var admissionIds = preview.Items.Select(x => x.StudentAdmissionId).ToHashSet();

            var admissions = new List<StudentAdmission>();
            foreach (var id in admissionIds)
            {
                var admission = await _studentAdmissionRepository.GetAdmissionByIdAsync(id, tenantId, branchId, cancellationToken);
                if (admission is null)
                    return ApiResponse<List<StudentAdmissionResponse>>.Fail($"Admission not found for id {id}.");

                admissions.Add(admission);
            }

            var previewMap = preview.Items.ToDictionary(x => x.StudentAdmissionId, x => x);

            foreach (var admission in admissions)
            {
                var item = previewMap[admission.Id];
                admission.SectionId = item.NewSectionId;
                admission.RollNo = item.NewRollNo;
                admission.UpdatedAt = DateTime.UtcNow;
                admission.UpdatedBy = actor;

                _studentAdmissionRepository.UpdateAdmission(admission);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = admissions.Select(admission =>
            {
                var item = previewMap[admission.Id];
                return new StudentAdmissionResponse
                {
                    Id = admission.Id,
                    StudentId = admission.StudentId,
                    AdmissionNo = admission.AdmissionNo,
                    AcademicYear = admission.AcademicYear?.Name ?? string.Empty,
                    ClassName = admission.AcademicClass?.Name ?? string.Empty,
                    SectionName = item.NewSectionName ?? string.Empty,
                    RollNo = admission.RollNo,
                    AdmissionDate = admission.AdmissionDate,
                    Status = admission.Status.ToString()
                };
            }).ToList();

            return ApiResponse<List<StudentAdmissionResponse>>.Ok(result, "Bulk section/roll assignment completed successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<List<StudentAdmissionResponse>>.Fail(ex.Message);
        }
    }

    private async Task<BulkSectionRollAssignmentPreviewResponse> BuildBulkAssignmentPreviewAsync(
        int tenantId,
        int branchId,
        BulkSectionRollAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Students is null || request.Students.Count == 0)
            throw new InvalidOperationException("Please select at least one student.");

        var academicYear = await _studentAdmissionRepository.GetAcademicYearAsync(
            request.AcademicYearId,
            tenantId,
            cancellationToken);

        if (academicYear is null)
            throw new InvalidOperationException("Academic year not found.");

        var academicClass = await _studentAdmissionRepository.GetAcademicClassAsync(
            request.ClassId,
            tenantId,
            cancellationToken);

        if (academicClass is null)
            throw new InvalidOperationException("Class not found.");

        Section? section = null;
        var currentStrength = 0;
        int? capacity = null;
        var availableSeats = 0;

        if (request.SectionId.HasValue)
        {
            section = await _studentAdmissionRepository.GetSectionAsync(
                request.SectionId.Value,
                tenantId,
                branchId,
                request.ClassId,
                cancellationToken);

            if (section is null)
                throw new InvalidOperationException("Selected section not found for class.");

            currentStrength = await _studentAdmissionRepository.GetSectionAdmissionCountAsync(
                tenantId,
                branchId,
                request.AcademicYearId,
                request.ClassId,
                section.Id,
                cancellationToken);

            capacity = section.Capacity;
        }

        var selectedAdmissions = new List<(StudentAdmission Admission, string StudentName, string? ManualRollNo)>();

        foreach (var row in request.Students)
        {
            var admission = await _studentAdmissionRepository.GetAdmissionByIdAsync(
                row.StudentAdmissionId,
                tenantId,
                branchId,
                cancellationToken);

            if (admission is null)
                throw new InvalidOperationException($"Admission not found for id {row.StudentAdmissionId}.");

            if (admission.AcademicYearId != request.AcademicYearId || admission.AcademicClassId != request.ClassId)
                throw new InvalidOperationException($"Admission {admission.AdmissionNo} does not belong to selected academic year/class.");

            var studentName = BuildStudentName(admission.Student);

            selectedAdmissions.Add((admission, studentName, row.ManualRollNo));
        }

        var movingIntoTargetSectionCount = selectedAdmissions.Count(x => x.Admission.SectionId != request.SectionId);

        if (request.SectionId.HasValue && capacity.HasValue)
        {
            var projectedStrength = currentStrength + movingIntoTargetSectionCount;
            if (projectedStrength > capacity.Value)
                throw new InvalidOperationException("Selected section capacity will be exceeded.");
        }

        availableSeats = capacity.HasValue
            ? Math.Max(0, capacity.Value - currentStrength)
            : 0;

        var ordered = request.Mode switch
        {
            SectionRollAssignmentMode.Alphabetical => request.Descending
                ? selectedAdmissions.OrderByDescending(x => x.StudentName).ToList()
                : selectedAdmissions.OrderBy(x => x.StudentName).ToList(),

            SectionRollAssignmentMode.AdmissionDate => request.Descending
                ? selectedAdmissions.OrderByDescending(x => x.Admission.AdmissionDate).ThenByDescending(x => x.Admission.Id).ToList()
                : selectedAdmissions.OrderBy(x => x.Admission.AdmissionDate).ThenBy(x => x.Admission.Id).ToList(),

            SectionRollAssignmentMode.AdmissionNo => request.Descending
                ? selectedAdmissions.OrderByDescending(x => x.Admission.AdmissionNo).ToList()
                : selectedAdmissions.OrderBy(x => x.Admission.AdmissionNo).ToList(),

            _ => selectedAdmissions
        };

        var existingRolls = await _studentAdmissionRepository.GetAssignedRollNumbersAsync(
            tenantId,
            branchId,
            request.AcademicYearId,
            request.ClassId,
            request.SectionId,
            null,
            cancellationToken);

        var usedRolls = new HashSet<string>(
            existingRolls.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()),
            StringComparer.OrdinalIgnoreCase);

        foreach (var admission in ordered)
        {
            if (!string.IsNullOrWhiteSpace(admission.Admission.RollNo) &&
                admission.Admission.SectionId == request.SectionId)
            {
                usedRolls.Remove(admission.Admission.RollNo.Trim());
            }
        }

        var numericExisting = existingRolls
            .Select(x => int.TryParse(x, out var n) ? (int?)n : null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();

        var nextNumber = request.StartFromRollNo.HasValue
            ? request.StartFromRollNo.Value
            : 1;

        if (numericExisting.Count > 0)
            nextNumber = Math.Max(nextNumber, numericExisting.Max() + 1);

        var previewItems = new List<BulkSectionRollAssignmentPreviewItemResponse>();
        var localAssigned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in ordered)
        {
            var previewItem = new BulkSectionRollAssignmentPreviewItemResponse
            {
                StudentAdmissionId = item.Admission.Id,
                StudentId = item.Admission.StudentId,
                StudentName = item.StudentName,
                AdmissionNo = item.Admission.AdmissionNo,
                AdmissionDate = item.Admission.AdmissionDate,
                CurrentSectionName = item.Admission.Section?.Name,
                CurrentRollNo = item.Admission.RollNo,
                NewSectionId = request.SectionId,
                NewSectionName = section?.Name
            };

            if (request.Mode == SectionRollAssignmentMode.Manual)
            {
                var manualRoll = string.IsNullOrWhiteSpace(item.ManualRollNo)
                    ? null
                    : item.ManualRollNo.Trim();

                previewItem.NewRollNo = manualRoll;

                if (string.IsNullOrWhiteSpace(manualRoll))
                {
                    previewItem.HasConflict = true;
                    previewItem.Message = "Manual roll number is required.";
                }
                else if (usedRolls.Contains(manualRoll) || localAssigned.Contains(manualRoll))
                {
                    previewItem.HasConflict = true;
                    previewItem.Message = "Duplicate roll number.";
                }
                else
                {
                    localAssigned.Add(manualRoll);
                }
            }
            else
            {
                while (usedRolls.Contains(nextNumber.ToString()) || localAssigned.Contains(nextNumber.ToString()))
                    nextNumber++;

                previewItem.NewRollNo = nextNumber.ToString();
                localAssigned.Add(previewItem.NewRollNo);
                nextNumber++;
            }

            previewItems.Add(previewItem);
        }

        return new BulkSectionRollAssignmentPreviewResponse
        {
            TotalStudents = previewItems.Count,
            CurrentStrength = currentStrength,
            Capacity = capacity,
            AvailableSeats = availableSeats,
            Items = previewItems
        };
    }


    private static string BuildStudentName(Student student)
    {
        var parts = new[]
        {
        student.FirstName,
        student.MiddleName,
        student.LastName
    };

        return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    }



    public async Task<ApiResponse<List<StudentAdmissionListItemResponse>>> GetAdmissionsByAcademicYearAndClassAsync(
    int tenantId,
    int branchId,
    int academicYearId,
    int classId,
    CancellationToken cancellationToken = default)
    {
        var academicYear = await _studentAdmissionRepository.GetAcademicYearAsync(
            academicYearId,
            tenantId,
            cancellationToken);

        if (academicYear is null)
            return ApiResponse<List<StudentAdmissionListItemResponse>>.Fail("Academic year not found.");

        var academicClass = await _studentAdmissionRepository.GetAcademicClassAsync(
            classId,
            tenantId,
            cancellationToken);

        if (academicClass is null)
            return ApiResponse<List<StudentAdmissionListItemResponse>>.Fail("Class not found.");

        var items = await _studentAdmissionRepository.GetAdmissionsByAcademicYearAndClassAsync(
            tenantId,
            branchId,
            academicYearId,
            classId,
            cancellationToken);

        return ApiResponse<List<StudentAdmissionListItemResponse>>.Ok(items);
    }
}