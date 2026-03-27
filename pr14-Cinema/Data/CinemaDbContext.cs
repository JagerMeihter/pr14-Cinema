using pr14_Cinema.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace pr14_Cinema.Data
{
    public class CinemaDbContext : DbContext
    {
        public CinemaDbContext() : base("name=CinemaDbContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<CinemaDbContext>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Отключаем каскадное удаление по всей базе
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // === Основные связи ===

            modelBuilder.Entity<Session>()
                .HasRequired(s => s.Movie)
                .WithMany(m => m.Sessions)
                .HasForeignKey(s => s.MovieId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Session>()
                .HasRequired(s => s.Hall)
                .WithMany(h => h.Sessions)
                .HasForeignKey(s => s.HallId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Seat>()
                .HasRequired(s => s.Hall)
                .WithMany(h => h.Seats)
                .HasForeignKey(s => s.HallId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.Session)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SessionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.Seat)
                .WithMany()                    // Seat не имеет коллекцию Tickets
                .HasForeignKey(t => t.SeatId)
                .WillCascadeOnDelete(false);

            // УДАЛЯЕМ Many-to-Many между Session и Seat!
            // Теперь занятость определяется через таблицу Tickets — это правильный подход.

            base.OnModelCreating(modelBuilder);
        }
    }
}