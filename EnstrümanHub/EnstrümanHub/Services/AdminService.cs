using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Apis.Auth.OAuth2;
using EnstrümanHub.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace EnstrümanHub.Services
{
    public class AdminService : IAdminService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminService> _logger;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly HashSet<string> _adminEmails;

        public AdminService(IConfiguration configuration, ILogger<AdminService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
            _jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");

            try
            {
                _adminEmails = new HashSet<string>(_configuration.GetSection("AdminEmails").Get<string[]>() ?? Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin emails from configuration");
                _adminEmails = new HashSet<string>();
            }
        }

        public async Task<(bool IsValid, string Token)> ValidateAdminLogin(string email, string password)
        {
            if (!_adminEmails.Contains(email))
            {
                return (false, string.Empty);
            }

            // TODO: Implement proper password validation
            if (email == "admin@gmail.com" && password == "123456")
            {
                var token = GenerateJwtToken(email);
                return (true, token);
            }

            return (false, string.Empty);
        }

        private string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<DashboardStats> GetDashboardStats()
        {
            // TODO: Implement real dashboard stats
            return new DashboardStats
            {
                TotalOrders = 0
            };
        }
    }
} 