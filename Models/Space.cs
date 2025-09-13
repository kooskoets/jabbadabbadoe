using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JabbadabbadoeBooking.Models;

public class Space
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal NightlyRate { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal CleaningFee { get; set; }

    public int MaxGuests { get; set; }

    public List<SpacePhoto> Photos { get; set; } = new();
}

public class SpacePhoto
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public int SpaceId { get; set; }
    public Space? Space { get; set; }
}
