namespace Shala.Shared.Common;

public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10; // -1 = All
    public string? SearchText { get; set; }

    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}