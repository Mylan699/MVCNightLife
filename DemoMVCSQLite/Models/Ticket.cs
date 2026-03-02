using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DemoMVCSQLite.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public int NombreDispo { get; set; }

        public int EventId { get; set; }

        // Navigation
        [ValidateNever]
        public Event Event { get; set; } = null!;
        [ValidateNever]
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}