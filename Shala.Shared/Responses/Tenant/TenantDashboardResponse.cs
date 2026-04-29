namespace Shala.Shared.Responses.Tenant;

public sealed class TenantDashboardResponse
{
    public string TenantName { get; set; } = string.Empty;
    public string SelectedBranchName { get; set; } = string.Empty;
    public string? SelectedBranchCode { get; set; }
    public bool IsAllBranches { get; set; }

    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int CurrentAdmissions { get; set; }
    public int TodayAdmissions { get; set; }

    public decimal TodayCollection { get; set; }
    public decimal MonthCollection { get; set; }
    public decimal TotalOutstanding { get; set; }

    public int DefaulterCount { get; set; }
    public int PendingDocuments { get; set; }
    public int LowStockItems { get; set; }

    public List<TenantDashboardTrendPointResponse> FeeTrend { get; set; } = new();
    public List<TenantDashboardRecentAdmissionResponse> RecentAdmissions { get; set; } = new();
    public List<TenantDashboardAlertResponse> Alerts { get; set; } = new();
}

public sealed class TenantDashboardTrendPointResponse
{
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public sealed class TenantDashboardRecentAdmissionResponse
{
    public int AdmissionId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public string AdmissionNo { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string AdmissionDateText { get; set; } = string.Empty;
}

public sealed class TenantDashboardAlertResponse
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "info";
}