using JabbadabbadoeBooking.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JabbadabbadoeBooking.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Space> Spaces => Set<Space>();
    public DbSet<SpacePhoto> SpacePhotos => Set<SpacePhoto>();
    // Laat staan als je Booking hebt in je project; anders kun je deze regel verwijderen.
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SpacePhoto>(entity =>
        {
            entity.HasOne(p => p.Space)
                  .WithMany(s => s.Photos)
                  .HasForeignKey(p => p.SpaceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
