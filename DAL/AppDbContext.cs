using Microsoft.EntityFrameworkCore;

namespace cub;

public class AppDbContext : DbContext
{
    public DbSet<Trip> Trips { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=cab47;User ID=SA;Password=7MyStrongPass123;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;");
    }
}
