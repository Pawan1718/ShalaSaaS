namespace Shala.Shared.Responses.Fees;

public class StudentChargeResponse
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int StudentAdmissionId { get; set; }
    public int StudentFeeAssignmentId { get; set; }
    public int FeeHeadId { get; set; }

    public string ChargeLabel { get; set; } = string.Empty;
    public string? PeriodLabel { get; set; }
    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FineAmount { get; set; }
    public decimal PaidAmount { get; set; }

    public decimal NetAmount { get; set; }
    public decimal BalanceAmount { get; set; }

    public bool IsSettled { get; set; }
    public bool IsCancelled { get; set; }
}