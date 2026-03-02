namespace DemoMVCSQLite.Models
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public string Biographie { get; set; } = string.Empty;

        // Navigation
        public ICollection<EventArtist> EventArtists { get; set; } = new List<EventArtist>();
    }
}