using FluentAssertions;
using MySvelteApp.Server.Application.Authentication.DTOs;
using Xunit;

namespace MySvelteApp.Server.Tests.Application.Authentication.DTOs;

public class RegisterRequestTests
{
    [Fact]
    public void RegisterRequest_DefaultValues_ShouldBeEmptyStrings()
    {
        // Act
        var request = new RegisterRequest();

        // Assert
        request.Username.Should().Be(string.Empty);
        request.Email.Should().Be(string.Empty);
        request.Password.Should().Be(string.Empty);
    }

    [Fact]
    public void RegisterRequest_WithValidValues_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        const string username = "testuser";
        const string email = "test@example.com";
        const string password = "testpassword";

        // Act
        var request = new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = password
        };

        // Assert
        request.Username.Should().Be(username);
        request.Email.Should().Be(email);
        request.Password.Should().Be(password);
    }

    [Fact]
    public void RegisterRequest_ShouldAllowNullValues()
    {
        // Act
        var request = new RegisterRequest
        {
            Username = null!,
            Email = null!,
            Password = null!
        };

        // Assert
        request.Username.Should().BeNull();
        request.Email.Should().BeNull();
        request.Password.Should().BeNull();
    }
}
