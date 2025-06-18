using Microsoft.AspNetCore.Mvc;

namespace EnstrümanHub.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return File("~/index.html", "text/html");
        }

        public IActionResult Products()
        {
            return File("~/products.html", "text/html");
        }

        public IActionResult ProductDetails(int id)
        {
            return File("~/product-details.html", "text/html");
        }

        public IActionResult Cart()
        {
            return File("~/cart.html", "text/html");
        }
    }
} 