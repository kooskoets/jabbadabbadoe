using System.ComponentModel.DataAnnotations;

namespace JabbadabbadoeBooking.Models;

public enum BookingStatus { Pending, Confirmed, Cancelled }

public class Booking
{
    public int Id { get; set; }

    [Required]
    public int SpaceId { get; set; }
    public Space? Space { get; set; }

    [Required, DataType(DataType.Date)]
    public DateOnly CheckIn { get; set; }

    [Required, DataType(DataType.Date)]
    public DateOnly CheckOut { get; set; }

    [Range(1, 10)]
    public int Guests { get; set; } = 1;

    // NAW
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = "";
    [Required, MaxLength(100)]
    public string LastName { get; set; } = "";
    [Required, MaxLength(200), EmailAddress]
    public string Email { get; set; } = "";
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Required, MaxLength(200)]
    public string Address { get; set; } = "";
    [Required, MaxLength(20)]
    public string PostalCode { get; set; } = "";
    [Required, MaxLength(100)]
    public string City { get; set; } = "";
    [Required, MaxLength(100)]
    public string Country { get; set; } = "";

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    [DataType(DataType.Currency)]
    public decimal TotalPrice { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public string? MolliePaymentId { get; set; }
    public string? CustomerPin { get; set; }
}