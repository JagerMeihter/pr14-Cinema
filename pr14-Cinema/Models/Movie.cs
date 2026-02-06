using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pr14_Cinema.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public string PosterUrl { get; set; }

        [Range(0, 10)]
        public double Rating { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string AgeRating { get; set; } // "12+", "16+", "18+"

        [Required]
        public int Duration { get; set; } // в минутах

        [Required]
        public string Genre { get; set; } // "Комедия, Драма, Фантастика"

        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}