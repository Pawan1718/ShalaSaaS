namespace Shala.Shared.Responses.Fees;

public sealed class FeeDashboardRowResponse
{
    public int StudentId { get; set; }
    public int AdmissionId { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;

    public string AcademicYear { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public string? RollNo { get; set; }

    public string? FeeStructureName { get; set; }

    public int ChargeCount { get; set; }
    public int PendingChargeCount { get; set; }
    public int ReceiptCount { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalBalance { get; set; }

    public bool HasAssignment { get; set; }
    public bool HasCharges { get; set; }
    public bool CanCollect { get; set; }

    public string WorkflowStatus { get; set; } = string.Empty;
    public DateTime? LastReceiptDate { get; set; }
}