namespace Shala.Shared.Responses.Fees;

public sealed class FeeLedgerRowResponse
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; }
    public string EntryType { get; set; } = string.Empty;

    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal RunningBalance { get; set; }

    public string? ReferenceNo { get; set; }
    public string? Remarks { get; set; }
}