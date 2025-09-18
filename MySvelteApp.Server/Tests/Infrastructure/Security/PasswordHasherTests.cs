using FluentAssertions;
using MySvelteApp.Server.Infrastructure.Security;
using Xunit;

namespace MySvelteApp.Server.Tests.Infrastructure.Security;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_ShouldReturnNonEmptyHashAndSalt()
    {
        // Arrange
        const string password = "TestPassword123";

        // Act
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        salt.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        salt.Should().NotBe(password);
    }

    [Fact]
    public void HashPassword_ShouldReturnBase64EncodedStrings()
    {
        // Arrange
        const string password = "TestPassword123";

        // Act
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Assert
        // Base64 strings should be valid and decodable
        Convert.FromBase64String(hash).Should().NotBeNull();
        Convert.FromBase64String(salt).Should().NotBeNull();
    }

    [Fact]
    public void HashPassword_ShouldGenerateDifferentHashesForSamePassword()
    {
        // Arrange
        const string password = "TestPassword123";

        // Act
        var (hash1, salt1) = _passwordHasher.HashPassword(password);
        var (hash2, salt2) = _passwordHasher.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2);
        salt1.Should().NotBe(salt2);
    }

    [Fact]
    public void HashPassword_ShouldGenerateDifferentSaltsForSamePassword()
    {
        // Arrange
        const string password = "TestPassword123";

        // Act
        var (_, salt1) = _passwordHasher.HashPassword(password);
        var (_, salt2) = _passwordHasher.HashPassword(password);

        // Assert
        salt1.Should().NotBe(salt2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        const string password = "TestPassword123";
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash, salt);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        const string correctPassword = "TestPassword123";
        const string wrongPassword = "WrongPassword456";
        var (hash, salt) = _passwordHasher.HashPassword(correctPassword);

        // Act
        var result = _passwordHasher.VerifyPassword(wrongPassword, hash, salt);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithCorrectPasswordCaseInsensitive_ShouldReturnFalse()
    {
        // Arrange
        const string password = "TestPassword123";
        const string wrongCasePassword = "testpassword123";
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(wrongCasePassword, hash, salt);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithSlightlyDifferentPassword_ShouldReturnFalse()
    {
        // Arrange
        const string password = "TestPassword123";
        const string slightlyDifferentPassword = "TestPassword124";
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(slightlyDifferentPassword, hash, salt);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        const string password = "TestPassword123";
        const string emptyPassword = "";
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(emptyPassword, hash, salt);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithNullPassword_ShouldThrow()
    {
        // Arrange
        const string password = "TestPassword123";
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Act
        Action act = () => _passwordHasher.VerifyPassword(null!, hash, salt);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void HashPassword_WithEmptyPassword_ShouldStillWork()
    {
        // Arrange
        const string password = "";

        // Act
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        salt.Should().NotBeNullOrEmpty();

        // Verify it can be verified
        var result = _passwordHasher.VerifyPassword(password, hash, salt);
        result.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_WithLongPassword_ShouldWork()
    {
        // Arrange
        string password = new string('A', 1000); // Very long password

        // Act
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        salt.Should().NotBeNullOrEmpty();

        // Verify it can be verified
        var result = _passwordHasher.VerifyPassword(password, hash, salt);
        result.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        const string password = "P@ssw0rd!#$%^&*()_+-=[]{}|;:,.<>?";

        // Act
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        salt.Should().NotBeNullOrEmpty();

        // Verify it can be verified
        var result = _passwordHasher.VerifyPassword(password, hash, salt);
        result.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_ShouldUseHMACSHA512()
    {
        // Arrange
        const string password = "TestPassword123";

        // Act
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Assert
        // HMAC-SHA512 produces 64-byte (512-bit) hashes
        var hashBytes = Convert.FromBase64String(hash);
        hashBytes.Length.Should().Be(64);

        // Salt is the HMAC key; default HMACSHA512 key length is 128 bytes
        var saltBytes = Convert.FromBase64String(salt);
        saltBytes.Length.Should().Be(128);
    }

    [Fact]
    public void VerifyPassword_WithTamperedHash_ShouldReturnFalse()
    {
        // Arrange
        const string password = "TestPassword123";
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Tamper with the hash
        var tamperedHash = hash.Replace(hash[0], 'X');

        // Act
        var result = _passwordHasher.VerifyPassword(password, tamperedHash, salt);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithTamperedSalt_ShouldReturnFalse()
    {
        // Arrange
        const string password = "TestPassword123";
        var (hash, salt) = _passwordHasher.HashPassword(password);

        // Tamper with the salt
        var tamperedSalt = salt.Replace(salt[0], 'X');

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash, tamperedSalt);

        // Assert
        result.Should().BeFalse();
    }
}
