namespace Shala.Shared.Requests.Fees;

public class FeeStructureItemRequest
{
    public int FeeHeadId { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public int FrequencyType { get; set; }
    public int? StartMonth { get; set; }
    public int? EndMonth { get; set; }
    public int? DueDay { get; set; }
    public int ApplyType { get; set; } = 2; // EveryYear default

    public bool IsOptional { get; set; }
    public bool IsActive { get; set; } = true;
}