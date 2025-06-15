namespace EnstrümanHub.Models
{
    public class PaginationResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int? PreviousPage => HasPreviousPage ? CurrentPage - 1 : null;
        public int? NextPage => HasNextPage ? CurrentPage + 1 : null;
        public int FirstItemIndex => ((CurrentPage - 1) * PageSize) + 1;
        public int LastItemIndex => Math.Min(CurrentPage * PageSize, TotalCount);

        public PaginationResult(List<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
} 