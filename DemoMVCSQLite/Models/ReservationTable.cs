using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DemoMVCSQLite.Models
{
    public class ReservationTable
    {
        public int ReservationTableId { get; set; }
        public DateTime DateReservation { get; set; } = DateTime.Now;
        public StatutReservation Statut { get; set; } = StatutReservation.EnAttente;

        // Clés étrangères
        public int ClientId { get; set; }
        public int TableVIPId { get; set; }
        public int EventId { get; set; }

        // Navigation
        [ValidateNever]
        public Client Client { get; set; } = null!;
        [ValidateNever]
        public TableVIP TableVIP { get; set; } = null!;
        [ValidateNever]
        public Event Event { get; set; } = null!;
    }
}