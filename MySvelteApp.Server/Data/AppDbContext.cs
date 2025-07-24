using Microsoft.EntityFrameworkCore;
using MySvelteApp.Server.Models.Auth;

namespace MySvelteApp.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
} 