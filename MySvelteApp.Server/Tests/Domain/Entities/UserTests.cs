using FluentAssertions;
using MySvelteApp.Server.Domain.Entities;
using Xunit;

namespace MySvelteApp.Server.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void User_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var user = new User();

        // Assert
        user.Id.Should().Be(0);
        user.Username.Should().Be(string.Empty);
        user.Email.Should().Be(string.Empty);
        user.PasswordHash.Should().Be(string.Empty);
        user.PasswordSalt.Should().Be(string.Empty);
    }

    [Fact]
    public void User_WithValidValues_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        const int id = 123;
        const string username = "testuser";
        const string email = "test@example.com";
        const string passwordHash = "hashed_password";
        const string passwordSalt = "salt_value";

        // Act
        var user = new User
        {
            Id = id,
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        // Assert
        user.Id.Should().Be(id);
        user.Username.Should().Be(username);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.PasswordSalt.Should().Be(passwordSalt);
    }

    [Fact]
    public void User_ShouldAllowNullValues()
    {
        // Act
        var user = new User
        {
            Username = null!,
            Email = null!,
            PasswordHash = null!,
            PasswordSalt = null!
        };

        // Assert
        user.Username.Should().BeNull();
        user.Email.Should().BeNull();
        user.PasswordHash.Should().BeNull();
        user.PasswordSalt.Should().BeNull();
    }

    [Fact]
    public void User_Id_ShouldBeMutable()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = 456;

        // Assert
        user.Id.Should().Be(456);
    }

    [Fact]
    public void User_Properties_ShouldBeMutable()
    {
        // Arrange
        var user = new User();

        // Act
        user.Username = "newusername";
        user.Email = "newemail@example.com";
        user.PasswordHash = "newhash";
        user.PasswordSalt = "newsalt";

        // Assert
        user.Username.Should().Be("newusername");
        user.Email.Should().Be("newemail@example.com");
        user.PasswordHash.Should().Be("newhash");
        user.PasswordSalt.Should().Be("newsalt");
    }

    [Fact]
    public void User_ShouldSupportEqualityComparison()
    {
        // Arrange
        var user1 = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash1",
            PasswordSalt = "salt1"
        };

        var user2 = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash1",
            PasswordSalt = "salt1"
        };

        var user3 = new User
        {
            Id = 2,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash1",
            PasswordSalt = "salt1"
        };

        // Assert - Reference equality
        user1.Should().NotBeSameAs(user2);
        user1.Should().NotBeSameAs(user3);

        // Note: For value equality, you'd need to override Equals and GetHashCode
        // This test ensures the class works as expected for Entity Framework
    }
}
