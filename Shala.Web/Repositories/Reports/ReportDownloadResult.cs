namespace Shala.Web.Repositories.Reports;

public sealed class ReportDownloadResult
{
    public bool IsSuccess { get; init; }
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = "report.csv";
    public string ContentType { get; init; } = "text/csv";
    public string? Message { get; init; }
}
