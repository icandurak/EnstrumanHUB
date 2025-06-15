using EnstrümanHub.Data;
using EnstrümanHub.Models;
using EnstrümanHub.Models.DTOs;
using EnstrümanHub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AuthUsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenGenerator _jwt;

    public AuthUsersController(ApplicationDbContext context, JwtTokenGenerator jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _context.AuthUsers.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
            return Conflict("Bu e-posta zaten kayıtlı.");

        var user = new AuthUser
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        _context.AuthUsers.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwt.GenerateToken(user);

        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Geçersiz e-posta veya şifre.");

        var token = _jwt.GenerateToken(user);

        return Ok(new { token });
    }
}
