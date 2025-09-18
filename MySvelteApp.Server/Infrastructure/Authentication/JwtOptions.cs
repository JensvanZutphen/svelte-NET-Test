using System.ComponentModel.DataAnnotations;

namespace MySvelteApp.Server.Infrastructure.Authentication;

/// <summary>
/// Public validator class for JwtOptions to support DataAnnotations validation.
/// </summary>
public static class JwtOptionsValidator
{
    /// <summary>
    /// Validates that a string value is not null, empty, or whitespace-only.
    /// </summary>
    public static ValidationResult ValidateNotWhitespace(string value, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            var displayName = context.DisplayName ?? context.MemberName ?? "Value";
            return new ValidationResult($"{displayName} cannot be blank or whitespace-only.");
        }
        return ValidationResult.Success!;
    }
}

public sealed class JwtOptions
{
    [Required]
    [MinLength(32, ErrorMessage = "Jwt:Key must be at least 32 characters long.")]
    public string Key { get; set; } = string.Empty;

    [Required]
    [CustomValidation(typeof(JwtOptionsValidator), nameof(JwtOptionsValidator.ValidateNotWhitespace))]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    [CustomValidation(typeof(JwtOptionsValidator), nameof(JwtOptionsValidator.ValidateNotWhitespace))]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Lifetime of the access token in hours.
    /// </summary>
    [Range(1, 168, ErrorMessage = "Jwt:AccessTokenLifetimeHours must be between 1 and 168 hours.")]
    public int AccessTokenLifetimeHours { get; set; } = 24;
}


