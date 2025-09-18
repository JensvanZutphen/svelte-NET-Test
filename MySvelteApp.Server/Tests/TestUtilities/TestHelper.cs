using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySvelteApp.Server.Domain.Entities;
using MySvelteApp.Server.Infrastructure.Authentication;
using MySvelteApp.Server.Infrastructure.Persistence;
using MySvelteApp.Server.Tests.TestUtilities;

namespace MySvelteApp.Server.Tests.TestUtilities;

public static class TestHelper
{
    public static AppDbContext CreateInMemoryDbContext(string dbName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }

    public static IOptions<JwtOptions> CreateJwtOptions() =>
        Options.Create(new JwtOptions
        {
            Key = TestData.Jwt.ValidKey,
            Issuer = TestData.Jwt.ValidIssuer,
            Audience = TestData.Jwt.ValidAudience,
            AccessTokenLifetimeHours = TestData.Jwt.ValidLifetimeHours
        });

    public static async Task SeedUsersAsync(AppDbContext context, params User[] users)
    {
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    public static void ClearDatabase(AppDbContext context)
    {
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();
    }
}
