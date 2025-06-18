using System.Threading.Tasks;
using EnstrümanHub.Models;

namespace EnstrümanHub.Services
{
    public interface IAdminService
    {
        Task<(bool IsValid, string Token)> ValidateAdminLogin(string email, string password);
        Task<DashboardStats> GetDashboardStats();
    }
} 