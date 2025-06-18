using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using EnstrümanHub.Services;
using Microsoft.Extensions.Logging;

namespace EnstrümanHub.Middleware
{
    public class FirebaseAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FirebaseAuthMiddleware> _logger;

        public FirebaseAuthMiddleware(
            RequestDelegate next, 
            IFirebaseAuthService firebaseAuthService,
            IConfiguration configuration,
            ILogger<FirebaseAuthMiddleware> logger)
        {
            _next = next;
            _firebaseAuthService = firebaseAuthService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("FirebaseAuthMiddleware başladı - Path: {Path}", context.Request.Path);

                StringValues authHeader;
                if (!context.Request.Headers.TryGetValue("Authorization", out authHeader))
                {
                    _logger.LogWarning("Authorization header bulunamadı");
                    await _next(context);
                    return;
                }

                string token = authHeader.ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token boş veya null");
                    await _next(context);
                    return;
                }

                _logger.LogInformation("Token alındı, doğrulanıyor...");
                string uid = await _firebaseAuthService.VerifyTokenAsync(token);
                _logger.LogInformation("Token doğrulandı, UID: {Uid}", uid);

                var user = await _firebaseAuthService.GetUserAsync(uid);
                _logger.LogInformation("Kullanıcı bilgileri alındı, Email: {Email}", user.Email);

                // Admin email listesini kontrol et
                var adminEmails = _configuration.GetSection("Admin:Emails").Get<string[]>() ?? Array.Empty<string>();
                _logger.LogInformation("Admin email listesi: {Emails}", string.Join(", ", adminEmails));

                if (adminEmails.Contains(user.Email))
                {
                    _logger.LogInformation("Kullanıcı admin listesinde bulundu");
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, uid),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var identity = new ClaimsIdentity(claims, "Firebase");
                    context.User = new ClaimsPrincipal(identity);
                    _logger.LogInformation("Kullanıcıya Admin rolü atandı");
                }
                else
                {
                    _logger.LogWarning("Kullanıcı admin listesinde bulunamadı: {Email}", user.Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token doğrulama hatası");
            }

            await _next(context);
        }
    }
} 