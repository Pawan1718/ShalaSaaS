namespace Shala.Shared.Requests.Fees;

public class CreateStudentFeeAssignmentRequest
{
    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }
    public int FeeStructureId { get; set; }

    public decimal DiscountAmount { get; set; }
    public decimal AdditionalChargeAmount { get; set; }
    public bool IsActive { get; set; } = true;
}