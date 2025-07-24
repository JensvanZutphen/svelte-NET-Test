using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySvelteApp.Server.Data;
using MySvelteApp.Server.Models.Auth;
using MySvelteApp.Server.Services;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace MySvelteApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            return BadRequest(new { message = "Username is taken." });

        CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = Convert.ToBase64String(passwordHash)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id.ToString(), user.Username);
        return Ok(new { token, userId = user.Id, username = user.Username });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
        if (user == null)
            return Unauthorized(new { message = "Invalid username or password." });

        if (!VerifyPasswordHash(model.Password, Convert.FromBase64String(user.PasswordHash)))
            return Unauthorized(new { message = "Invalid username or password." });

        var token = _jwtService.GenerateToken(user.Id.ToString(), user.Username);
        return Ok(new { token, userId = user.Id, username = user.Username });
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] storedHash)
    {
        using (var hmac = new HMACSHA512())
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
} 