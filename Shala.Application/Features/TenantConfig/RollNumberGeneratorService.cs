using Shala.Application.Repositories.Students;
using Shala.Application.Repositories.TenantConfig;
using Shala.Domain.Entities.Students;

namespace Shala.Application.Features.TenantConfig;

public class RollNumberGeneratorService : IRollNumberGeneratorService
{
    private readonly IRollNumberSettingRepository _rollNumberSettingRepository;
    private readonly IStudentAdmissionRepository _studentAdmissionRepository;

    public RollNumberGeneratorService(
        IRollNumberSettingRepository rollNumberSettingRepository,
        IStudentAdmissionRepository studentAdmissionRepository)
    {
        _rollNumberSettingRepository = rollNumberSettingRepository;
        _studentAdmissionRepository = studentAdmissionRepository;
    }

    public async Task<string?> ResolveRollNoForCreateAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        string? requestedRollNo,
        CancellationToken cancellationToken = default)
    {
        var setting = await _rollNumberSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        var sanitizedRequestedRollNo = Normalize(requestedRollNo);

        if (setting is null)
        {
            if (string.IsNullOrWhiteSpace(sanitizedRequestedRollNo))
                return null;

            var existsWithoutSetting = await _studentAdmissionRepository.RollNoExistsAsync(
                tenantId,
                branchId,
                academicYearId,
                classId,
                sectionId,
                sanitizedRequestedRollNo,
                null,
                cancellationToken);

            if (existsWithoutSetting)
                throw new InvalidOperationException("Roll number already exists for the selected scope.");

            return sanitizedRequestedRollNo;
        }

        if (!setting.AutoGenerate)
        {
            if (string.IsNullOrWhiteSpace(sanitizedRequestedRollNo))
                return null;

            var manualExists = await _studentAdmissionRepository.RollNoExistsAsync(
                tenantId,
                branchId,
                academicYearId,
                classId,
                sectionId,
                sanitizedRequestedRollNo,
                null,
                cancellationToken);

            if (manualExists)
                throw new InvalidOperationException("Roll number already exists for the selected scope.");

            return sanitizedRequestedRollNo;
        }

        if (setting.AllowManualOverride && !string.IsNullOrWhiteSpace(sanitizedRequestedRollNo))
        {
            var manualOverrideExists = await _studentAdmissionRepository.RollNoExistsAsync(
                tenantId,
                branchId,
                academicYearId,
                classId,
                sectionId,
                sanitizedRequestedRollNo,
                null,
                cancellationToken);

            if (manualOverrideExists)
                throw new InvalidOperationException("Roll number already exists for the selected scope.");

            return sanitizedRequestedRollNo;
        }

        var generatedRollNo = await GenerateAsync(
            setting,
            tenantId,
            branchId,
            academicYearId,
            classId,
            sectionId,
            null,
            cancellationToken);

        return generatedRollNo;
    }

    public async Task<string?> ResolveRollNoForUpdateAsync(
        int tenantId,
        int branchId,
        int academicYearId,
        int classId,
        int? sectionId,
        int admissionId,
        int oldAcademicYearId,
        int oldClassId,
        int? oldSectionId,
        string? requestedRollNo,
        CancellationToken cancellationToken = default)
    {
        var setting = await _rollNumberSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        var sanitizedRequestedRollNo = Normalize(requestedRollNo);

        if (setting is null)
        {
            if (string.IsNullOrWhiteSpace(sanitizedRequestedRollNo))
                return null;

            var existsWithoutSetting = await _studentAdmissionRepository.RollNoExistsAsync(
                tenantId,
                branchId,
                academicYearId,
                classId,
                sectionId,
                sanitizedRequestedRollNo,
                admissionId,
                cancellationToken);

            if (existsWithoutSetting)
                throw new InvalidOperationException("Roll number already exists for the selected scope.");

            return sanitizedRequestedRollNo;
        }

        if (!setting.AutoGenerate)
        {
            if (string.IsNullOrWhiteSpace(sanitizedRequestedRollNo))
                return null;

            var manualExists = await _studentAdmissionRepository.RollNoExistsAsync(
                tenantId,
                branchId,
                academicYearId,
                classId,
                sectionId,
                sanitizedRequestedRollNo,
                admissionId,
                cancellationToken);

            if (manualExists)
                throw new InvalidOperationException("Roll number already exists for the selected scope.");

            return sanitizedRequestedRollNo;
        }

        if (setting.AllowManualOverride && !string.IsNullOrWhiteSpace(sanitizedRequestedRollNo))
        {
            var manualOverrideExists = await _studentAdmissionRepository.RollNoExistsAsync(
                tenantId,
                branchId,
                academicYearId,
                classId,
                sectionId,
                sanitizedRequestedRollNo,
                admissionId,
                cancellationToken);

            if (manualOverrideExists)
                throw new InvalidOperationException("Roll number already exists for the selected scope.");

            return sanitizedRequestedRollNo;
        }

        var scopeChanged =
            oldAcademicYearId != academicYearId ||
            oldClassId != classId ||
            oldSectionId != sectionId;

        if (!scopeChanged && !string.IsNullOrWhiteSpace(sanitizedRequestedRollNo))
        {
            var requestedSameScopeExists = await _studentAdmissionRepository.RollNoExistsAsync(
                tenantId,
                branchId,
                academicYearId,
                classId,
                sectionId,
                sanitizedRequestedRollNo,
                admissionId,
                cancellationToken);

            if (requestedSameScopeExists)
                throw new InvalidOperationException("Roll number already exists for the selected scope.");

            return sanitizedRequestedRollNo;
        }

        var generatedRollNo = await GenerateAsync(
            setting,
            tenantId,
            branchId,
            academicYearId,
            classId,
            sectionId,
            admissionId,
            cancellationToken);

        return generatedRollNo;
    }

    private async Task<string> GenerateAsync(
     RollNumberSetting setting,
     int tenantId,
     int branchId,
     int academicYearId,
     int classId,
     int? sectionId,
     int? excludeAdmissionId,
     CancellationToken cancellationToken)
    {
        if (setting.NumberPadding < 1)
            throw new InvalidOperationException("Invalid roll number padding configuration.");

        if (string.IsNullOrWhiteSpace(setting.Format))
            throw new InvalidOperationException("Roll number format is not configured.");

        var existingRolls = await _studentAdmissionRepository.GetAssignedRollNumbersAsync(
            tenantId,
            branchId,
            academicYearId,
            classId,
            sectionId,
            excludeAdmissionId,
            cancellationToken);

        var numericRolls = existingRolls
            .Select(x => int.TryParse(x, out var number) ? (int?)number : null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();

        var nextNumber = numericRolls.Count == 0
            ? setting.StartFrom
            : Math.Max(setting.StartFrom, numericRolls.Max() + 1);

        var candidate = ApplyFormat(
            setting.Format,
            setting.Prefix,
            nextNumber,
            setting.NumberPadding,
            classId,
            sectionId,
            academicYearId);

        var exists = await _studentAdmissionRepository.RollNoExistsAsync(
            tenantId,
            branchId,
            academicYearId,
            classId,
            sectionId,
            candidate,
            excludeAdmissionId,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Unable to generate a unique roll number. Please try again.");

        return candidate;
    }

    private static string ApplyFormat(
        string format,
        string? prefix,
        int number,
        int padding,
        int classId,
        int? sectionId,
        int academicYearId)
    {
        var padded = number.ToString().PadLeft(padding, '0');

        return format
            .Replace("{prefix}", prefix ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("{number}", padded, StringComparison.OrdinalIgnoreCase)
            .Replace("{class}", classId.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("{section}", sectionId?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("{year}", academicYearId.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}