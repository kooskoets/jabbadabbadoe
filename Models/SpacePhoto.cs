using System;

namespace JabbadabbadoeBooking.Models;

public class SpacePhoto
{
    public int Id { get; set; }
    public int SpaceId { get; set; }

    /// <summary>Bestandsnaam op schijf (zonder pad)</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>Relatief pad voor <img src>, bv. /images/spaces/{SpaceId}/{FileName}</summary>
    public string? RelativePath { get; set; }

    /// <summary>
    /// Compatibele alias die in views/controllers gebruikt kan zijn.
    /// Leest/schrijft naar RelativePath.
    /// </summary>
    public string FilePath
    {
        get => RelativePath ?? $"/images/spaces/{SpaceId}/{FileName}";
        set => RelativePath = value;
    }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigatie
    public Space? Space { get; set; }
}
