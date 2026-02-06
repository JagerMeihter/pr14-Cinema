using pr14_Cinema.Models;
using System.Data.Entity;

namespace pr14_Cinema.Data
{
    public class CinemaDbContext : DbContext
    {
        public CinemaDbContext() : base("name=CinemaDbContext")
        {
            Configuration.LazyLoadingEnabled = true;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Настройка связей
            modelBuilder.Entity<Session>()
                .HasRequired(s => s.Movie)
                .WithMany(m => m.Sessions)
                .HasForeignKey(s => s.MovieId);

            modelBuilder.Entity<Session>()
                .HasRequired(s => s.Hall)
                .WithMany(h => h.Sessions)
                .HasForeignKey(s => s.HallId);

            modelBuilder.Entity<Seat>()
                .HasRequired(s => s.Hall)
                .WithMany(h => h.Seats)
                .HasForeignKey(s => s.HallId);

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.Session)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SessionId);

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.Seat)
                .WithMany()
                .HasForeignKey(t => t.SeatId);

            // Многие ко многим для Session и Seat (доступные места)
            modelBuilder.Entity<Session>()
                .HasMany(s => s.AvailableSeats)
                .WithMany(s => s.Sessions)
                .Map(m =>
                {
                    m.ToTable("SessionSeats");
                    m.MapLeftKey("SessionId");
                    m.MapRightKey("SeatId");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}