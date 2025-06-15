namespace EnstrümanHub.Models
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public enum ProductSortField
    {
        Name,
        Price,
        Brand,
        Category,
        CreatedAt
    }

    public class SortOptions
    {
        public ProductSortField Field { get; set; }
        public SortDirection Direction { get; set; }

        public static SortOptions Default => new()
        {
            Field = ProductSortField.Name,
            Direction = SortDirection.Ascending
        };
    }
} 