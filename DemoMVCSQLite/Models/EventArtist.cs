namespace DemoMVCSQLite.Models
{
    public class EventArtist
    {
        public int EventId { get; set; }
        public int ArtistId { get; set; }
        public DateTime HeurePassage { get; set; }

        // Navigation
        public Event Event { get; set; } = null!;
        public Artist Artist { get; set; } = null!;
    }
}