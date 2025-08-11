using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace JabbadabbadoeBooking.Services;

public class BookingService
{
    private readonly ApplicationDbContext _db;
    public BookingService(ApplicationDbContext db) { _db = db; }

    public async Task<bool> IsAvailableAsync(int spaceId, DateOnly checkIn, DateOnly checkOut, int? excludeBookingId = null)
    {
        if (checkOut <= checkIn) return false;
        var overlaps = await _db.Bookings
            .Where(b => b.SpaceId == spaceId && b.Status != BookingStatus.Cancelled)
            .Where(b => excludeBookingId == null || b.Id != excludeBookingId.Value)
            .Where(b => !(b.CheckOut <= checkIn || b.CheckIn >= checkOut))
            .AnyAsync();
        return !overlaps;
    }

    public decimal CalculateTotalPrice(Space space, DateOnly checkIn, DateOnly checkOut)
    {
        var nights = checkOut.DayNumber - checkIn.DayNumber;
        if (nights < 1) nights = 1;
        return (space.NightlyRate * nights) + space.CleaningFee;
    }

    public string GeneratePin() => new Random().Next(100000, 999999).ToString();
}