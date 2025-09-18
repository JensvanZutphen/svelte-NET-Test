using System.ComponentModel.DataAnnotations;

namespace MySvelteApp.Server.Infrastructure.Authentication;

public sealed class JwtOptions
{
    [Required]
    [MinLength(32, ErrorMessage = "Jwt:Key must be at least 32 characters long.")]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Lifetime of the access token in hours.
    /// </summary>
    [Range(1, 168, ErrorMessage = "Jwt:AccessTokenLifetimeHours must be between 1 and 168 hours.")]
    public int AccessTokenLifetimeHours { get; set; } = 24;
}


