using Shala.Domain.Enums;

namespace Shala.Shared.Responses.Supplies;

public class StudentSupplyIssueResponse
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }
    public string IssueNo { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public SupplyPaymentStatus PaymentStatus { get; set; }
    public string? Remarks { get; set; }
    public List<StudentSupplyIssueItemResponse> Items { get; set; } = new();
    public List<StudentSupplyPaymentResponse> Payments { get; set; } = new();
}