using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MySvelteApp.Server.Domain.Entities;
using MySvelteApp.Server.Infrastructure.Authentication;
using MySvelteApp.Server.Tests.TestUtilities;
using Xunit;

namespace MySvelteApp.Server.Tests.Infrastructure.Authentication;

public class JwtTokenGeneratorTests
{
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public JwtTokenGeneratorTests()
    {
        _jwtOptions = TestHelper.CreateJwtOptions();
        _jwtTokenGenerator = new JwtTokenGenerator(_jwtOptions);
    }

    [Fact]
    public void GenerateToken_WithValidUser_ShouldReturnValidJwtToken()
    {
        // Arrange
        var user = TestData.Users.ValidUser;

        // Act
        var token = _jwtTokenGenerator.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be(_jwtOptions.Value.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtOptions.Value.Audience);
        jwtToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddHours(_jwtOptions.Value.AccessTokenLifetimeHours), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectClaims()
    {
        // Arrange
        var user = TestData.Users.ValidUser;

        // Act
        var token = _jwtTokenGenerator.GenerateToken(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);

        claims[ClaimTypes.NameIdentifier].Should().Be(user.Id.ToString());
        claims[JwtRegisteredClaimNames.Sub].Should().Be(user.Id.ToString());
        claims[ClaimTypes.Name].Should().Be(user.Username);
        claims[JwtRegisteredClaimNames.Jti].Should().NotBeNullOrEmpty();
        claims[JwtRegisteredClaimNames.Iat].Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_ShouldHaveUniqueJtiForEachToken()
    {
        // Arrange
        var user = TestData.Users.ValidUser;

        // Act
        var token1 = _jwtTokenGenerator.GenerateToken(user);
        var token2 = _jwtTokenGenerator.GenerateToken(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken1 = tokenHandler.ReadJwtToken(token1);
        var jwtToken2 = tokenHandler.ReadJwtToken(token2);

        var jti1 = jwtToken1.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var jti2 = jwtToken2.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        jti1.Should().NotBe(jti2);
    }

    [Fact]
    public void GenerateToken_WithDifferentUsers_ShouldHaveDifferentClaims()
    {
        // Arrange
        var user1 = TestData.Users.ValidUser;
        var user2 = TestData.Users.AnotherValidUser;

        // Act
        var token1 = _jwtTokenGenerator.GenerateToken(user1);
        var token2 = _jwtTokenGenerator.GenerateToken(user2);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken1 = tokenHandler.ReadJwtToken(token1);
        var jwtToken2 = tokenHandler.ReadJwtToken(token2);

        var claims1 = jwtToken1.Claims.ToDictionary(c => c.Type, c => c.Value);
        var claims2 = jwtToken2.Claims.ToDictionary(c => c.Type, c => c.Value);

        claims1[ClaimTypes.NameIdentifier].Should().NotBe(claims2[ClaimTypes.NameIdentifier]);
        claims1[ClaimTypes.Name].Should().NotBe(claims2[ClaimTypes.Name]);
    }

    [Fact]
    public void GenerateToken_WithBase64Key_ShouldWorkCorrectly()
    {
        // Arrange
        var jwtOptions = Options.Create(new JwtOptions
        {
            Key = TestData.Jwt.ValidKey, // base64 encoded key
            Issuer = TestData.Jwt.ValidIssuer,
            Audience = TestData.Jwt.ValidAudience,
            AccessTokenLifetimeHours = TestData.Jwt.ValidLifetimeHours
        });

        var generator = new JwtTokenGenerator(jwtOptions);
        var user = TestData.Users.ValidUser;

        // Act
        var token = generator.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be(jwtOptions.Value.Issuer);
        jwtToken.Audiences.Should().Contain(jwtOptions.Value.Audience);
    }

    [Fact]
    public void GenerateToken_WithPlainTextKey_ShouldWorkCorrectly()
    {
        // Arrange
        var longPlainKey = "ThisIsAVeryLongPlainTextKeyThatIsDefinitelyLongerThan32CharactersForTesting";
        var jwtOptions = Options.Create(new JwtOptions
        {
            Key = longPlainKey,
            Issuer = TestData.Jwt.ValidIssuer,
            Audience = TestData.Jwt.ValidAudience,
            AccessTokenLifetimeHours = TestData.Jwt.ValidLifetimeHours
        });

        var generator = new JwtTokenGenerator(jwtOptions);
        var user = TestData.Users.ValidUser;

        // Act
        var token = generator.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be(jwtOptions.Value.Issuer);
        jwtToken.Audiences.Should().Contain(jwtOptions.Value.Audience);
    }

    [Fact]
    public void GenerateToken_ShouldHaveCorrectExpirationTime()
    {
        // Arrange
        var user = TestData.Users.ValidUser;
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _jwtTokenGenerator.GenerateToken(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var expectedExpiration = beforeGeneration.AddHours(_jwtOptions.Value.AccessTokenLifetimeHours);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void GenerateToken_ShouldUseHmacSha256Algorithm()
    {
        // Arrange
        var user = TestData.Users.ValidUser;

        // Act
        var token = _jwtTokenGenerator.GenerateToken(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken.SignatureAlgorithm.Should().Be("HS256");
    }
}
