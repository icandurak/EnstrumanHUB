using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnstrümanHub.Models;
using EnstrümanHub.Services;

namespace EnstrümanHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // POST: api/admin/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (isValid, token) = await _adminService.ValidateAdminLogin(request.Email, request.Password);
            
            if (!isValid)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new { token });
        }

        // GET: api/admin/dashboard
        [Authorize]
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _adminService.GetDashboardStats();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting dashboard stats", error = ex.Message });
            }
        }

        // GET: api/admin/verify
        [Authorize]
        [HttpGet("verify")]
        public ActionResult VerifyAdmin()
        {
            return Ok(new { IsValid = true });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AdminAuthResponse
    {
        public string Token { get; set; }
    }

    public class DashboardStats
    {
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> RecentOrders { get; set; }
    }
} 