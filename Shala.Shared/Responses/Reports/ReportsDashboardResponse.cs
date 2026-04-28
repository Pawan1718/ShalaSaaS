namespace Shala.Shared.Responses.Reports;

public sealed class ReportsDashboardResponse
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TodayAdmissions { get; set; }
    public int CurrentAdmissions { get; set; }

    public decimal TodayCollection { get; set; }
    public decimal MonthCollection { get; set; }
    public decimal TotalOutstanding { get; set; }

    public int DefaulterCount { get; set; }
    public int PendingDocuments { get; set; }
}
