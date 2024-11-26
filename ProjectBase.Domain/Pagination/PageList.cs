namespace ProjectBase.Domain.Pagination
{
    public class PageList<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRow { get; set; }
        public int PageCount => (int)Math.Ceiling(TotalRow * 1.0 / PageSize);
        public bool HasPrev => PageIndex > 1;
        public bool HasNext => PageIndex < PageCount - 1;
        public IEnumerable<T?> PageData { get; set; } = [];
    }
}
