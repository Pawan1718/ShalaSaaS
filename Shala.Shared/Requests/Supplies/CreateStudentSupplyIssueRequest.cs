using Shala.Domain.Enums;

namespace Shala.Shared.Requests.Supplies;

public class CreateStudentSupplyIssueRequest
{
    public int AcademicYearId { get; set; }
    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }
    public decimal PaidAmount { get; set; }
    public SupplyPaymentMode PaymentMode { get; set; } = SupplyPaymentMode.Cash;
    public string? ReferenceNo { get; set; }
    public string? Remarks { get; set; }
    public List<CreateStudentSupplyIssueItemRequest> Items { get; set; } = new();
}