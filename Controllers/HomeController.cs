using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JabbadabbadoeBooking.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) { _db = db; }

    public async Task<IActionResult> Index(int year = 0, int month = 0)
    {
        var space = await _db.Spaces.FirstAsync();
        if (year == 0 || month == 0) { year = DateTime.Today.Year; month = DateTime.Today.Month; }
        var first = new DateOnly(year, month, 1);
        var next = first.AddMonths(1);
        var bookings = await _db.Bookings
            .Where(b => b.SpaceId == space.Id && b.Status != BookingStatus.Cancelled)
            .Where(b => !(b.CheckOut <= first || b.CheckIn >= next))
            .ToListAsync();
        ViewBag.Space = space;
        ViewBag.Month = month;
        ViewBag.Year = year;
        ViewBag.Bookings = bookings;
        return View();
    }
}