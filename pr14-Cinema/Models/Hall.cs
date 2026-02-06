using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pr14_Cinema.Models
{
    public class Hall
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } // "Standard", "VIP", "IMAX", "3D"

        public string Description { get; set; }

        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}