using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Models;
using JabbadabbadoeBooking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mollie.Api.Client;

namespace JabbadabbadoeBooking.Controllers;

[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly IEmailSender _email;
    public PaymentsController(ApplicationDbContext db, IConfiguration config, IEmailSender email)
    {
        _db = db; _config = config; _email = email;
    }

    [HttpPost("payments/webhook")]
    public async Task<IActionResult> Webhook([FromForm] string id)
    {
        var client = new PaymentClient(_config["Mollie:ApiKey"]);
        var payment = await client.GetPaymentAsync(id);

        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.MolliePaymentId == id);
        if (booking == null) return Ok();

        var status = payment.Status?.ToLowerInvariant();

        if (status == "paid")
        {
            if (booking.Status != BookingStatus.Confirmed)
            {
                booking.Status = BookingStatus.Confirmed;
                await _db.SaveChangesAsync();
                await _email.SendAsync(
                    booking.Email,
                    "Bevestiging reservering — Jabbadabbadoe",
                    $"<p>Je betaling is ontvangen. Reservering #{booking.Id} is bevestigd.</p>");
            }
        }
        else if (status == "canceled" || status == "expired" || status == "failed")
        {
            booking.Status = BookingStatus.Cancelled;
            await _db.SaveChangesAsync();
        }

        return Ok();

    }
}