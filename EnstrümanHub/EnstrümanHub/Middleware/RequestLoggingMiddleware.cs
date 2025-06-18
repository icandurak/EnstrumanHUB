using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EnstrümanHub.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // İstek bilgilerini logla
            var request = context.Request;
            var requestBody = string.Empty;

            // POST, PUT gibi isteklerin body'sini oku
            if (request.Method == "POST" || request.Method == "PUT")
            {
                request.EnableBuffering();
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }
            }

            _logger.LogInformation(
                "API İsteği: {Method} {Path} - Body: {Body}",
                request.Method,
                request.Path,
                requestBody
            );

            // Yanıtı yakalamak için orijinal response stream'i sakla
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);

                    // Yanıt bilgilerini logla
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var response = await new StreamReader(responseBody).ReadToEndAsync();
                    responseBody.Seek(0, SeekOrigin.Begin);

                    _logger.LogInformation(
                        "API Yanıtı: {StatusCode} - Body: {Body}",
                        context.Response.StatusCode,
                        response
                    );

                    await responseBody.CopyToAsync(originalBodyStream);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "API isteği sırasında hata oluştu");
                    throw;
                }
            }
        }
    }
} 