namespace Shala.Shared.Responses.Reports;

public sealed class StudentFeeLedgerReportResponse
{
    public int LedgerId { get; set; }
    public int StudentId { get; set; }
    public int AdmissionId { get; set; }

    public DateTime EntryDate { get; set; }
    public string EntryType { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal RunningBalance { get; set; }

    public string? ReferenceNo { get; set; }
    public string? Remarks { get; set; }
}
