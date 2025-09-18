using Microsoft.EntityFrameworkCore;
using MySvelteApp.Server.Domain.Entities;

namespace MySvelteApp.Server.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();
        user.Property(u => u.Username).IsRequired().HasMaxLength(64);
        user.Property(u => u.Email).IsRequired().HasMaxLength(320);
        user.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512);
        user.Property(u => u.PasswordSalt).IsRequired().HasMaxLength(512);

        user.HasIndex(u => u.Username).IsUnique();
        user.HasIndex(u => u.Email).IsUnique();

        // Align SQLite with case-insensitive email semantics
        // This requires Microsoft.EntityFrameworkCore.Sqlite package
        // Check if SQLite provider is available at runtime
        if (Database.ProviderName?.Contains("Sqlite") == true)
        {
            try
            {
                // Use reflection to avoid compile-time dependency on SQLite package
                var propertyBuilder = user.Property(u => u.Email);
                var useCollationMethod = propertyBuilder.GetType().GetMethod("UseCollation");
                if (useCollationMethod is not null)
                {
                    useCollationMethod.Invoke(propertyBuilder, ["NOCASE"]);
                }
            }
            catch
            {
                // Silently ignore if SQLite collation is not available
                // The database will still work with case-sensitive comparison
            }
        }
    }
}
