using Microsoft.EntityFrameworkCore;

namespace cub;

public class AppDbContext : DbContext
{
    public DbSet<Trip> Trips { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=db;User ID=SA;Password=YourPassword;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;");
    }
}
