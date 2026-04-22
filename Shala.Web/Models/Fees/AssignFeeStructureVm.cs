namespace Shala.Web.Models.Fees;

public class AssignFeeStructureVm
{
    public int FeeStructureId { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal AdditionalChargeAmount { get; set; }
    public bool AutoGenerateCharges { get; set; } = true;
}