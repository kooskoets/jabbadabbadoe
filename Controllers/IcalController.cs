using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Models;

namespace JabbadabbadoeBooking.Controllers;

public class IcalController : Controller
{
    private readonly ApplicationDbContext _db;
    public IcalController(ApplicationDbContext db) { _db = db; }

    [HttpGet("/ical/space.ics")]
    public async Task<IActionResult> Export()
    {
        var space = await _db.Spaces.FirstAsync();
        var bookings = await _db.Bookings
            .Where(b => b.SpaceId == space.Id && b.Status == BookingStatus.Confirmed)
            .OrderBy(b => b.CheckIn).ToListAsync();

        var cal = new Calendar();
        cal.AddProperty("X-WR-CALNAME", "Jabbadabbadoe — Bezetting");
        foreach (var b in bookings)
        {
            var e = new CalendarEvent
            {
                Summary = $"Bezet — Booking #{b.Id}",
                // all-day: use Y/M/D ctor, HasTime=false
                DtStart = new CalDateTime(b.CheckIn.Year, b.CheckIn.Month, b.CheckIn.Day),
                DtEnd = new CalDateTime(b.CheckOut.Year, b.CheckOut.Month, b.CheckOut.Day),
            };

            cal.Events.Add(e);
        }

        var serializer = new CalendarSerializer();
        var ics = serializer.SerializeToString(cal) ?? string.Empty;
        return File(System.Text.Encoding.UTF8.GetBytes(ics), "text/calendar", "space.ics");
    }
}
