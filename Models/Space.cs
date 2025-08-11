using System.ComponentModel.DataAnnotations;

namespace JabbadabbadoeBooking.Models;

public class Space
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [Required, MaxLength(4000)]
    public string Description { get; set; } = "";

    public int MaxGuests { get; set; }

    [DataType(DataType.Currency)]
    public decimal NightlyRate { get; set; }

    [DataType(DataType.Currency)]
    public decimal CleaningFee { get; set; }

    public string PhotoList { get; set; } = "";
}