namespace Shala.Shared.Responses.Fees;

public class StudentFeeAssignmentResponse
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }
    public int FeeStructureId { get; set; }

    public string FeeStructureName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;

    public decimal DiscountAmount { get; set; }
    public decimal AdditionalChargeAmount { get; set; }

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}