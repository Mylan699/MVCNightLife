using Microsoft.EntityFrameworkCore;
using DemoMVCSQLite.Models;

namespace DemoMVCSQLite.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<EventArtist> EventArtists { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<TableVIP> TablesVIP { get; set; }
        public DbSet<ReservationTable> ReservationsTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Clé composite pour EventArtist (relation N-N)
            modelBuilder.Entity<EventArtist>()
                .HasKey(ea => new { ea.EventId, ea.ArtistId });

            // Relations EventArtist
            modelBuilder.Entity<EventArtist>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.EventArtists)
                .HasForeignKey(ea => ea.EventId);

            modelBuilder.Entity<EventArtist>()
                .HasOne(ea => ea.Artist)
                .WithMany(a => a.EventArtists)
                .HasForeignKey(ea => ea.ArtistId);
        }
    }
}