using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Models;

namespace JabbadabbadoeBooking.Services;

public class PaymentService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(ApplicationDbContext db, ILogger<PaymentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Demo-implementatie: geen Mollie; stuur direct terug naar PaymentReturn met demo=1
    public Task<string> CreatePaymentAsync(Booking booking, HttpRequest request)
    {
        _logger.LogInformation("PaymentService stub: demo redirect voor booking {Id}", booking.Id);
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var url = $"{baseUrl}/Bookings/PaymentReturn?id={booking.Id}&demo=1";
        return Task.FromResult(url);
    }
}
