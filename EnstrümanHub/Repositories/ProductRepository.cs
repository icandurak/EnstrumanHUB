using EnstrümanHub.Models;
using Microsoft.EntityFrameworkCore;

namespace EnstrümanHub.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Product> Products, int TotalCount)> GetFilteredProductsAsync(ProductFilterDto filter)
        {
            var query = _context.Products.AsQueryable();

            // Apply filters
            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filter.Brand))
                query = query.Where(p => p.Brand == filter.Brand);

            if (!string.IsNullOrWhiteSpace(filter.Category))
                query = query.Where(p => p.Category == filter.Category);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(p => p.Name.Contains(filter.SearchTerm) || 
                                       p.Description.Contains(filter.SearchTerm));

            if (filter.InStock.HasValue)
                query = query.Where(p => p.StockQuantity > 0 == filter.InStock.Value);

            // Apply sorting
            query = ApplySorting(query, filter.GetSortOptions());

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var products = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        private IQueryable<Product> ApplySorting(IQueryable<Product> query, SortOptions sortOptions)
        {
            return sortOptions.Field switch
            {
                ProductSortField.Name => sortOptions.Direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Name)
                    : query.OrderByDescending(p => p.Name),

                ProductSortField.Price => sortOptions.Direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Price)
                    : query.OrderByDescending(p => p.Price),

                ProductSortField.Brand => sortOptions.Direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Brand)
                    : query.OrderByDescending(p => p.Brand),

                ProductSortField.Category => sortOptions.Direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Category)
                    : query.OrderByDescending(p => p.Category),

                ProductSortField.CreatedAt => sortOptions.Direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.CreatedAt)
                    : query.OrderByDescending(p => p.CreatedAt),

                _ => query.OrderBy(p => p.Name)
            };
        }

        public async Task<List<string>> GetAllBrandsAsync()
        {
            return await _context.Products
                .Select(p => p.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
} 