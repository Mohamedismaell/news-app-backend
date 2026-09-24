namespace NewsBackend.Application.DTOs.Common;

public class PagedResponse<T>
{
    public PagedResponse()
    {
    }

    public PagedResponse(IReadOnlyList<T> items, int page, int limit, int totalCount)
    {
        Items = items;
        Page = page;
        Limit = limit;
        TotalCount = totalCount;
        TotalPages = limit > 0 ? (int)Math.Ceiling(totalCount / (double)limit) : 0;
    }

    public int Page { get; set; }

    public int Limit { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public IReadOnlyList<T> Items { get; set; } = [];
}