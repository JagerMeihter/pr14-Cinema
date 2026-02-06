using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace pr14_Cinema.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        [Required]
        public int HallId { get; set; }
        public virtual Hall Hall { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public decimal TicketPrice { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Seat> AvailableSeats { get; set; } = new List<Seat>();
    }
}