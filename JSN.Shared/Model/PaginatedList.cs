namespace JSN.Shared.Model;

public class PaginatedList<T>
{
    public PaginatedList(List<T>? items, int count, int pageIndex, int pageSize)
    {
        TotalPage = (int)Math.Ceiling(count / (double)pageSize);
        Data = items!;
        PageIndex = pageIndex;
    }

    public int PageIndex { get; set; }
    public int TotalPage { get; set; }
    public List<T> Data { get; set; }
}