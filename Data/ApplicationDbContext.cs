using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JabbadabbadoeBooking.Models;

namespace JabbadabbadoeBooking.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Space> Spaces => Set<Space>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Space)
            .WithMany()
            .HasForeignKey(b => b.SpaceId);

        modelBuilder.Entity<Booking>()
            .Property(b => b.CheckIn)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>();
        modelBuilder.Entity<Booking>()
            .Property(b => b.CheckOut)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>();
    }
}

public class DateOnlyConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d)) { }
}
public class DateOnlyComparer : Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<DateOnly>
{
    public DateOnlyComparer() : base((d1, d2) => d1 == d2, d => d.GetHashCode()) { }
}

public static class Seed
{
    public static void SeedData(ApplicationDbContext db, IConfiguration config)
    {
        if (!db.Spaces.Any())
        {
            var title = config["Site:ListingTitle"] ?? "Jabbadabbadoe — Studio";
            var nightly = decimal.Parse(config["Site:NightlyRate"] ?? "95");
            var clean = decimal.Parse(config["Site:CleaningFee"] ?? "25");
            var maxGuests = int.Parse(config["Site:MaxGuests"] ?? "2");

            db.Spaces.Add(new Space
            {
                Title = title,
                Description = "Knusse studio van 30m² met 12m² overkapping, privé-ingang, kitchenette, BBQ op patio, 15 min. wandelen naar centrum.",
                MaxGuests = maxGuests,
                NightlyRate = nightly,
                CleaningFee = clean,
                PhotoList = "placeholder1.jpg,placeholder2.jpg,placeholder3.jpg"
            });
            db.SaveChanges();
        }
    }
}