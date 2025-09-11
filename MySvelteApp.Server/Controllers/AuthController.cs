using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySvelteApp.Server.Data;
using MySvelteApp.Server.Models.Auth;
using MySvelteApp.Server.Services;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace MySvelteApp.Server.Controllers;

// Response types for OpenAPI documentation
public class AuthSuccessResponse
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class AuthErrorResponse
{
    public string Message { get; set; } = string.Empty;
}

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
    [ProducesResponseType(typeof(AuthSuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(model.Username))
            return BadRequest(new { message = "Username is required." });

        if (model.Username.Length < 3)
            return BadRequest(new { message = "Username must be at least 3 characters long." });

        if (!System.Text.RegularExpressions.Regex.IsMatch(model.Username, "^[a-zA-Z0-9_]+$"))
            return BadRequest(new { message = "Username can only contain letters, numbers, and underscores." });

        if (string.IsNullOrWhiteSpace(model.Email))
            return BadRequest(new { message = "Email is required." });

        if (!System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            return BadRequest(new { message = "Please enter a valid email address." });

        if (string.IsNullOrWhiteSpace(model.Password))
            return BadRequest(new { message = "Password is required." });

        if (model.Password.Length < 8)
            return BadRequest(new { message = "Password must be at least 8 characters long." });

        if (!System.Text.RegularExpressions.Regex.IsMatch(model.Password, @"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)"))
            return BadRequest(new { message = "Password must contain at least one uppercase letter, one lowercase letter, and one number." });

        // Check if username is already taken
        if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            return BadRequest(new { message = "This username is already taken. Please choose a different one." });

        // Check if email is already registered
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            return BadRequest(new { message = "This email is already registered. Please use a different email address." });

        CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = model.Username.Trim(),
            Email = model.Email.Trim().ToLower(),
            PasswordHash = Convert.ToBase64String(passwordHash),
            PasswordSalt = Convert.ToBase64String(passwordSalt)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id.ToString(), user.Username);
        return Ok(new { token, userId = user.Id, username = user.Username });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthSuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(model.Username))
            return BadRequest(new { message = "Username is required." });

        if (string.IsNullOrWhiteSpace(model.Password))
            return BadRequest(new { message = "Password is required." });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username.Trim());
        if (user == null)
            return Unauthorized(new { message = "Invalid username or password. Please check your credentials and try again." });

        if (!VerifyPasswordHash(model.Password, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.PasswordSalt)))
            return Unauthorized(new { message = "Invalid username or password. Please check your credentials and try again." });

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

    private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
} 