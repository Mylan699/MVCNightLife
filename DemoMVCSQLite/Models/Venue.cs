namespace DemoMVCSQLite.Models
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public int Capacite { get; set; }
        public string Description { get; set; } = string.Empty;

        // Navigation
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<TableVIP> TablesVIP { get; set; } = new List<TableVIP>();
    }
}