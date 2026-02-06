using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public string Type { get; set; } // "Standard", "VIP", "Disabled"

        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}