using Microsoft.EntityFrameworkCore;
using MySvelteApp.Server.Domain.Entities;

namespace MySvelteApp.Server.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}
