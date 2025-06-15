using EnstrümanHub.Models;
using EnstrümanHub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EnstrümanHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredProducts([FromQuery] ProductFilterDto filter)
        {
            var (products, totalCount) = await _productRepository.GetFilteredProductsAsync(filter);
            
            var paginationResult = new PaginationResult<Product>(
                products,
                totalCount,
                filter.Page,
                filter.PageSize
            );

            return Ok(paginationResult);
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _productRepository.GetAllBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _productRepository.GetAllCategoriesAsync();
            return Ok(categories);
        }
    }
} 