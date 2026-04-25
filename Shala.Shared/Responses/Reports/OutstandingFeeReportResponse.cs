namespace Shala.Shared.Responses.Reports;

public sealed class OutstandingFeeReportResponse
{
    public int StudentId { get; set; }
    public int AdmissionId { get; set; }

    public string AdmissionNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;

    public string AcademicYearName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public string? RollNo { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalFine { get; set; }
    public decimal NetAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
}
