using MySvelteApp.Server.Application.Authentication.DTOs;

namespace MySvelteApp.Server.Presentation.Models.Auth;

public class AuthErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public AuthErrorType ErrorType { get; set; } = AuthErrorType.None;
}
