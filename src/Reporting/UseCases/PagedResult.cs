namespace XEvent.Reporting;

public record PagedResult<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount, 
    Dictionary<string, string> Links
);