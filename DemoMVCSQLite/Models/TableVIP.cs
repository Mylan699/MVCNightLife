using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DemoMVCSQLite.Models
{
    public class TableVIP
    {
        public int TableVIPId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int NombrePlaces { get; set; }
        public decimal Prix { get; set; }

        // Clé étrangère
        public int VenueId { get; set; }

        // Navigation
        [ValidateNever]
        public Venue Venue { get; set; } = null!;
        [ValidateNever]
        public ICollection<ReservationTable> ReservationsTables { get; set; } = new List<ReservationTable>();
    }
}