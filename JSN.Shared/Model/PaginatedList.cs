namespace JSN.Shared.Model;

public class PaginatedList<T>(List<T>? items, int count, int pageIndex, int pageSize)
{
    public int PageIndex { get; set; } = pageIndex;
    public int TotalPage { get; set; } = (int)Math.Ceiling(count / (double)pageSize);
    public List<T> Data { get; set; } = items!;
}