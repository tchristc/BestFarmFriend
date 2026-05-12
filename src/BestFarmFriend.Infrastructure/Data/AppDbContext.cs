using BestFarmFriend.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BestFarmFriend.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Crop> Crops => Set<Crop>();
    public DbSet<CropAction> CropActions => Set<CropAction>();
    public DbSet<GrowthStageEntry> GrowthStageEntries => Set<GrowthStageEntry>();
    public DbSet<CalendarTask> CalendarTasks => Set<CalendarTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Crop>()
            .HasMany(c => c.Actions)
            .WithOne()
            .HasForeignKey(a => a.CropId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Crop>()
            .HasMany(c => c.GrowthStages)
            .WithOne()
            .HasForeignKey(g => g.CropId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Location>()
            .HasIndex(l => new { l.Latitude, l.Longitude });
    }
}
