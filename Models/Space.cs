using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

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

    /// <summary>
    /// Komma-gescheiden lijst met bestandsnamen, bv. "foto1.jpg,foto2.jpg".
    /// Wordt door seed-code en oudere views gebruikt.
    /// </summary>
    public string? PhotoList { get; set; }

    /// <summary>
    /// Optionele navigatie als je met aparte entiteiten werkt.
    /// Niet vereist voor compilatie; laat staan als je SpacePhoto gebruikt.
    /// </summary>
    public List<SpacePhoto> Photos { get; set; } = new();
}
