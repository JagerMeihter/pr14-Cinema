using System;
using System.ComponentModel.DataAnnotations;

namespace pr14_Cinema.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public int SessionId { get; set; }
        public virtual Session Session { get; set; }

        [Required]
        public int SeatId { get; set; }
        public virtual Seat Seat { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } // "Active", "Used", "Cancelled"
    }
}