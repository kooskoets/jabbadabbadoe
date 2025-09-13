using Microsoft.EntityFrameworkCore;
using JabbadabbadoeBooking.Models;

namespace JabbadabbadoeBooking.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Space> Spaces => Set<Space>();
    public DbSet<SpacePhoto> SpacePhotos => Set<SpacePhoto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Space>()
            .HasMany(s => s.Photos)
            .WithOne(p => p.Space)
            .HasForeignKey(p => p.SpaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
