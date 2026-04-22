using Shala.Shared.Responses.Fees;

namespace Shala.Web.Models.Fees;

public class AdmissionFeeDashboardVm
{
    public StudentFeeAssignmentResponse? Assignment { get; set; }

    public List<FeeStructureResponse> AvailableStructures { get; set; } = new();
    public List<StudentChargeResponse> Charges { get; set; } = new();
    public List<FeeReceiptResponse> Receipts { get; set; } = new();

    public FeeSummaryVm Summary { get; set; } = new();
}