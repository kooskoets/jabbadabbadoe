using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Models;
using JabbadabbadoeBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JabbadabbadoeBooking.Controllers;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly BookingService _svc;
    private readonly PaymentService? _payments;

    public BookingsController(ApplicationDbContext db, BookingService svc, IServiceProvider services)
    {
        _db = db;
        _svc = svc;
        _payments = services.GetService<PaymentService>();
    }

    // ---------- Helper: cleanup oude Pending boekingen ----------
    private async Task<int> CleanupStalePendingAsync(int minutes = 60)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-minutes);
        var stale = await _db.Bookings
            .Where(b => b.Status == BookingStatus.Pending && b.CreatedAtUtc < cutoff)
            .ToListAsync();

        foreach (var b in stale)
            b.Status = BookingStatus.Cancelled;

        if (stale.Count > 0)
            await _db.SaveChangesAsync();

        return stale.Count;
    }
    // ------------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> Create(DateOnly? checkIn, DateOnly? checkOut)
    {
        await CleanupStalePendingAsync(); // opruimen bij binnenkomst

        var space = await _db.Spaces.FirstAsync();
        ViewBag.Space = space;

        var model = new Booking
        {
            SpaceId = space.Id,
            CheckIn = checkIn ?? DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOut = checkOut ?? DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Guests = 1,
            Country = "Nederland"
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking model)
    {
        var space = await _db.Spaces.FirstAsync();
        ViewBag.Space = space;

        if (model.CheckOut <= model.CheckIn)
            ModelState.AddModelError(string.Empty, "Check-out moet na check-in liggen.");
        if (model.Guests > space.MaxGuests)
            ModelState.AddModelError(nameof(model.Guests), $"Maximaal {space.MaxGuests} gasten.");
        if (!await _svc.IsAvailableAsync(space.Id, model.CheckIn, model.CheckOut))
            ModelState.AddModelError(string.Empty, "Deze periode is niet beschikbaar.");
        if (!ModelState.IsValid) return View(model);

        model.SpaceId = space.Id;
        model.TotalPrice = _svc.CalculateTotalPrice(space, model.CheckIn, model.CheckOut);
        model.Status = BookingStatus.Pending;

        using var tx = await _db.Database.BeginTransactionAsync();
        if (!await _svc.IsAvailableAsync(space.Id, model.CheckIn, model.CheckOut))
        {
            await tx.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Net geboekt: deze periode is zojuist bezet geraakt. Kies andere data.");
            return View(model);
        }

        _db.Bookings.Add(model);
        await _db.SaveChangesAsync();

        string payUrl;
        if (_payments is not null)
            payUrl = await _payments.CreatePaymentAsync(model, Request);
        else
            payUrl = Url.Action(nameof(PaymentReturn), "Bookings", new { id = model.Id, demo = true }, Request.Scheme)!;

        await tx.CommitAsync();
        return Redirect(payUrl);
    }

    [HttpGet]
    public async Task<IActionResult> PaymentReturn(int id, bool demo = false)
    {
        await CleanupStalePendingAsync();

        var booking = await _db.Bookings.Include(b => b.Space).FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null) return NotFound();

        if (demo && booking.Status == BookingStatus.Pending)
        {
            booking.Status = BookingStatus.Confirmed;
            await _db.SaveChangesAsync();
        }

        return View(booking);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var booking = await _db.Bookings.Include(b => b.Space).FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null) return NotFound();

        if (ViewBag.PinOk == null) ViewBag.PinOk = false;
        return View(booking);
    }

    // ----- Admin acties -----
    [HttpGet]
    [Authorize(AuthenticationSchemes = "AdminBasic", Policy = "AdminOnly")]
    public async Task<IActionResult> AdminList()
    {
        var cleaned = await CleanupStalePendingAsync();
        if (cleaned > 0) TempData["Message"] = $"{cleaned} oude pending reserveringen automatisch geannuleerd.";

        var items = await _db.Bookings.Include(b => b.Space)
            .OrderByDescending(b => b.CreatedAtUtc).ToListAsync();
        return View(items);
    }

    // Markeer als betaald (alleen voor demo/test of handmatige correctie)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = "AdminBasic", Policy = "AdminOnly")]
    public async Task<IActionResult> MarkPaid(int id)
    {
        var b = await _db.Bookings.FindAsync(id);
        if (b == null) return NotFound();

        if (b.Status == BookingStatus.Pending)
        {
            b.Status = BookingStatus.Confirmed;
            await _db.SaveChangesAsync();
            TempData["Message"] = "Reservering gemarkeerd als betaald.";
        }
        return RedirectToAction(nameof(AdminList));
    }

    // Bestaande cancel-acties kunnen blijven staan
}
