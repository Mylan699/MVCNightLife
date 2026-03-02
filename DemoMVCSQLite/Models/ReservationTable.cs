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
        public Client Client { get; set; } = null!;
        public TableVIP TableVIP { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}