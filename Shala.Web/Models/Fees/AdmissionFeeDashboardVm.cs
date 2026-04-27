using Shala.Shared.Responses.Fees;

namespace Shala.Web.Models.Fees;

public class AdmissionFeeDashboardVm
{
    public List<StudentFeeAssignmentResponse> Assignments { get; set; } = new();

    public StudentFeeAssignmentResponse? Assignment =>
        Assignments.FirstOrDefault(x => x.IsActive) ?? Assignments.FirstOrDefault();

    public List<FeeStructureResponse> AvailableStructures { get; set; } = new();
    public List<StudentChargeResponse> Charges { get; set; } = new();
    public List<FeeReceiptResponse> Receipts { get; set; } = new();

    public FeeSummaryVm Summary { get; set; } = new();
}