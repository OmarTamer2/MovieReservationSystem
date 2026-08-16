
namespace MovieReservationSystem.Dtos
{
    public class MovieDto
    {
        public Guid Id {get; set;}
        public string Title { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }

        public string? genre {get; set;} = "";
    }
}