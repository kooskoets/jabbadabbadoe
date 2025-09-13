using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JabbadabbadoeBooking.Models
{
    /// <summary>
    /// Enige, definitieve versie van SpacePhoto.
    /// Let op: er is slechts één key naar Space: SpaceId (int).
    /// </summary>
    public class SpacePhoto
    {
        public int Id { get; set; }

        [Required]
        public int SpaceId { get; set; }   // <-- dit is de enige foreign key eigenschap

        [Required, MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [ForeignKey(nameof(SpaceId))]
        public Space? Space { get; set; }
    }
}
