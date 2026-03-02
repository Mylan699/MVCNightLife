namespace DemoMVCSQLite.Models
{
    public enum StatutReservation { EnAttente, Confirmee, Annulee }

    public class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime DateReservation { get; set; } = DateTime.Now;
        public StatutReservation Statut { get; set; } = StatutReservation.EnAttente;
        public int QteTickets { get; set; }
        public decimal MontantTotal { get; set; }

        // Clés étrangères
        public int ClientId { get; set; }
        public int TicketId { get; set; }

        // Navigation
        public Client Client { get; set; } = null!;
        public Ticket Ticket { get; set; } = null!;
    }
}