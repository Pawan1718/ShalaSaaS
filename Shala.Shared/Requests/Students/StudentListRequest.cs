using System.ComponentModel.DataAnnotations;

namespace Shala.Shared.Requests.Students;

public class StudentListRequest
{
    public string? Search { get; set; }
    public int? AcademicYearId { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 10;
}