
namespace MovieReservationSystem.Models
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string Description { get; set; } = "";
        public Guid genreId {get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }
    }
}