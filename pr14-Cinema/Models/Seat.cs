using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pr14_Cinema.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HallId { get; set; }
        public virtual Hall Hall { get; set; }

        [Required]
        public int Row { get; set; }

        [Required]
        public int Number { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; }   // Standard, VIP, Disabled

        // ==================== Свойства ТОЛЬКО для UI (не сохраняются в БД) ====================
        [NotMapped]
        public bool IsOccupied { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        [NotMapped]
        public string DisplayName => $"Ряд {Row}, место {Number}";
    }
}