using Shala.Shared.Enums;

namespace Shala.Shared.Requests.Students;

public class BulkSectionRollAssignmentStudentItemRequest
{
    public int StudentAdmissionId { get; set; }
    public string? ManualRollNo { get; set; }
}

public class BulkSectionRollAssignmentRequest
{
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public int? SectionId { get; set; }

    public SectionRollAssignmentMode Mode { get; set; } = SectionRollAssignmentMode.AutoSequential;

    public int? StartFromRollNo { get; set; }
    public bool Descending { get; set; }

    public List<BulkSectionRollAssignmentStudentItemRequest> Students { get; set; } = new();
}