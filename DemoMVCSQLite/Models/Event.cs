using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DemoMVCSQLite.Models
{
    public enum StatutEvent { Planifie, EnCours, Termine, Annule }

    public class Event
    {
        public int EventId { get; set; }
        public string Titre { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string ThemeMusical { get; set; } = string.Empty;
        public decimal PrixEntree { get; set; }
        public int CapaciteMax { get; set; }
        public StatutEvent Statut { get; set; } = StatutEvent.Planifie;

        public int VenueId { get; set; }

        // Navigation
        [ValidateNever]
        public Venue Venue { get; set; } = null!;
        [ValidateNever]
        public ICollection<EventArtist> EventArtists { get; set; } = new List<EventArtist>();
        [ValidateNever]
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        [ValidateNever]
        public ICollection<ReservationTable> ReservationsTables { get; set; } = new List<ReservationTable>();
    }
}