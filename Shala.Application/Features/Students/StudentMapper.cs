using Shala.Domain.Entities.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;

namespace Shala.Application.Features.Students;

public static class StudentMapper
{
    public static StudentDetailsResponse MapStudentDetails(Student student)
    {
        var currentAdmission = student.Admissions
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.AdmissionDate)
            .FirstOrDefault();

        return new StudentDetailsResponse
        {
            Id = student.Id,
            TenantId = student.TenantId,
            BranchId = student.BranchId,
            AdmissionNo = currentAdmission?.AdmissionNo,
            FirstName = student.FirstName,
            MiddleName = student.MiddleName,
            LastName = student.LastName,
            FullName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Replace("  ", " ").Trim(),
            Gender = student.Gender.ToString(),
            DateOfBirth = student.DateOfBirth,
            AadhaarNo = student.AadhaarNo,
            BloodGroup = student.BloodGroup,
            Mobile = student.Mobile,
            Email = student.Email,
            Address = student.Address,
            PhotoUrl = student.PhotoUrl,
            Status = student.Status.ToString(),
            Guardians = student.Guardians.Select(g => new GuardianResponse
            {
                Id = g.Id,
                Name = g.Name,
                RelationType = g.RelationType.ToString(),
                Mobile = g.Mobile,
                Email = g.Email,
                IsPrimary = g.IsPrimary
            }).ToList(),
            Admissions = student.Admissions.Select(a => new StudentAdmissionResponse
            {
                Id = a.Id,
                StudentId = a.StudentId,
                AdmissionNo = a.AdmissionNo,
                AcademicYear = a.AcademicYear?.Name ?? string.Empty,
                ClassName = a.AcademicClass?.Name ?? string.Empty,
                SectionName = a.Section?.Name ?? string.Empty,
                RollNo = a.RollNo,
                AdmissionDate = a.AdmissionDate,
                Status = a.Status.ToString()
            }).ToList()
        };
    }

    public static StudentListItemResponse MapStudentListItem(Student student)
    {
        var currentAdmission = student.Admissions
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.AdmissionDate)
            .FirstOrDefault();

        return new StudentListItemResponse
        {
            Id = student.Id,
            AdmissionNo = currentAdmission?.AdmissionNo ?? string.Empty,
            FullName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Replace("  ", " ").Trim(),
            Gender = student.Gender.ToString(),
            DateOfBirth = student.DateOfBirth,
            Mobile = student.Mobile,
            Status = student.Status.ToString(),
            CurrentAcademicYear = currentAdmission?.AcademicYear?.Name,
            CurrentClass = currentAdmission?.AcademicClass?.Name,
            CurrentSection = currentAdmission?.Section?.Name
        };
    }
}