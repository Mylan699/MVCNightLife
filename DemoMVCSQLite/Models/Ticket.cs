namespace DemoMVCSQLite.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string Type { get; set; } = string.Empty; // Standard, VIP, VVIP
        public decimal Prix { get; set; }
        public int NombreDispo { get; set; }

        // Clé étrangère
        public int EventId { get; set; }

        // Navigation
        public Event Event { get; set; } = null!;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}