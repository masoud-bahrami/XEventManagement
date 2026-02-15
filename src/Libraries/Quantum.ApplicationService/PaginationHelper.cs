public record PageInfo(int Page, int PageSize, int TotalCount);

public static class PaginationHelper
{
    /// <summary>
    /// Paginate a list and generate HATEOAS links
    /// </summary>
    public static (List<T> Items, Dictionary<string, string> Links)
        PaginateAndLink<T>(
            IEnumerable<T> source,
            PageInfo pageInfo,
            string baseUrl,
            Dictionary<string, string>? additionalQuery = null)
    {
        var skip = (pageInfo.Page - 1) * pageInfo.PageSize;
        var items = source.Skip(skip).Take(pageInfo.PageSize).ToList();

        var links = new Dictionary<string, string>
        {
            // HATEOAS Links
            ["self"] = $"{baseUrl}?{QueryString(pageInfo.Page)}",
            ["first"] = $"{baseUrl}?{QueryString(1)}",
            ["last"] = $"{baseUrl}?{QueryString((int) Math.Ceiling((double) pageInfo.TotalCount / pageInfo.PageSize))}"
        };

        if (pageInfo.Page > 1)
            links["prev"] = $"{baseUrl}?{QueryString(pageInfo.Page - 1)}";

        if ((pageInfo.Page * pageInfo.PageSize) < pageInfo.TotalCount)
            links["next"] = $"{baseUrl}?{QueryString(pageInfo.Page + 1)}";

        return (items, links);


        string QueryString(int page)
        {
            var baseParams = new[] {("page", page.ToString()), ("pageSize", pageInfo.PageSize.ToString())};

            var additionalParams = (additionalQuery ?? new Dictionary<string, string>())
                .Select(kv => (kv.Key, kv.Value));

            return string.Join("&", baseParams.Concat(additionalParams).Select(kv => $"{kv.Item1}={kv.Item2}"));
        }
    }
}