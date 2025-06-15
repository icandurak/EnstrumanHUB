namespace EnstrümanHub.Models
{
    public class ProductFilterDto
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string SearchTerm { get; set; }
        public bool? InStock { get; set; }
        public ProductSortField SortField { get; set; } = ProductSortField.Name;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public SortOptions GetSortOptions()
        {
            return new SortOptions
            {
                Field = SortField,
                Direction = SortDirection
            };
        }
    }
} 