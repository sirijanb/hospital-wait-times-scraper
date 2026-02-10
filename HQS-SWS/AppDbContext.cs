using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<ScrapedData> ScrapedHospitalData { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}