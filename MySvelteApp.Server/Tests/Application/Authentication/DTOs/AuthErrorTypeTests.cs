using FluentAssertions;
using MySvelteApp.Server.Application.Authentication.DTOs;
using Xunit;

namespace MySvelteApp.Server.Tests.Application.Authentication.DTOs;

public class AuthErrorTypeTests
{
    [Fact]
    public void AuthErrorType_ShouldHaveCorrectValues()
    {
        // Assert
        ((int)AuthErrorType.None).Should().Be(0);
        ((int)AuthErrorType.Validation).Should().Be(1);
        ((int)AuthErrorType.Conflict).Should().Be(2);
        ((int)AuthErrorType.Unauthorized).Should().Be(3);
    }

    [Fact]
    public void AuthErrorType_ShouldHaveCorrectNames()
    {
        // Assert
        AuthErrorType.None.ToString().Should().Be("None");
        AuthErrorType.Validation.ToString().Should().Be("Validation");
        AuthErrorType.Conflict.ToString().Should().Be("Conflict");
        AuthErrorType.Unauthorized.ToString().Should().Be("Unauthorized");
    }

    [Fact]
    public void AuthErrorType_ShouldBeConvertibleToInt()
    {
        // Act & Assert
        int noneValue = (int)AuthErrorType.None;
        int validationValue = (int)AuthErrorType.Validation;
        int conflictValue = (int)AuthErrorType.Conflict;
        int unauthorizedValue = (int)AuthErrorType.Unauthorized;

        noneValue.Should().Be(0);
        validationValue.Should().Be(1);
        conflictValue.Should().Be(2);
        unauthorizedValue.Should().Be(3);
    }

    [Fact]
    public void AuthErrorType_ShouldBeConvertibleFromInt()
    {
        // Act & Assert
        ((AuthErrorType)0).Should().Be(AuthErrorType.None);
        ((AuthErrorType)1).Should().Be(AuthErrorType.Validation);
        ((AuthErrorType)2).Should().Be(AuthErrorType.Conflict);
        ((AuthErrorType)3).Should().Be(AuthErrorType.Unauthorized);
    }
}
