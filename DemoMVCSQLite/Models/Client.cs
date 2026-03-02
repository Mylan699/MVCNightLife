namespace DemoMVCSQLite.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public DateTime DateNaissance { get; set; }

        // Navigation
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<ReservationTable> ReservationsTables { get; set; } = new List<ReservationTable>();
    }
}