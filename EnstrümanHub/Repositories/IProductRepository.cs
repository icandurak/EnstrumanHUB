using EnstrümanHub.Models;

namespace EnstrümanHub.Repositories
{
    public interface IProductRepository
    {
        Task<(List<Product> Products, int TotalCount)> GetFilteredProductsAsync(ProductFilterDto filter);
        Task<List<string>> GetAllBrandsAsync();
        Task<List<string>> GetAllCategoriesAsync();
    }
} 