using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EnstrümanHub.Middleware;
using EnstrümanHub.Services;
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Firebase servislerini ekle
builder.Services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>();

// Firestore servisini ekle
builder.Services.AddSingleton<IAdminService, AdminService>();

// Firebase ve Firestore servislerini ekle
builder.Services.AddSingleton<FirebaseAuthService>();
builder.Services.AddSingleton<FirestoreDb>(provider =>
{
    var projectId = builder.Configuration["Firebase:ProjectId"];
    var credentialsPath = builder.Configuration["Firebase:CredentialsPath"];
    
    if (string.IsNullOrEmpty(projectId))
    {
        throw new InvalidOperationException("Firebase:ProjectId configuration is missing");
    }
    
    if (string.IsNullOrEmpty(credentialsPath))
    {
        throw new InvalidOperationException("Firebase:CredentialsPath configuration is missing");
    }

    if (!File.Exists(credentialsPath))
    {
        throw new InvalidOperationException($"Firebase credentials file not found at: {credentialsPath}");
    }

    var credentials = Google.Apis.Auth.OAuth2.GoogleCredential
        .FromFile(credentialsPath)
        .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

    return new FirestoreDbBuilder
    {
        ProjectId = projectId,
        Credential = credentials
    }.Build();
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Authorization politikası ekle
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Logging'i yapılandır
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Register services
builder.Services.AddScoped<IAdminService, AdminService>();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var ex = error.Error;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    });
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseStaticFiles();

// Default dosyaları servis et
app.UseDefaultFiles();

// API isteklerini logla
app.UseMiddleware<RequestLoggingMiddleware>();

// Firebase middleware'ini ekle (Authentication'dan önce)
app.UseMiddleware<FirebaseAuthMiddleware>();

// Authentication ve Authorization middleware'lerini ekle
app.UseAuthentication();
app.UseAuthorization();

// API endpoint'lerini yapılandır
app.MapControllers();

// Frontend route'larını yapılandır
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
