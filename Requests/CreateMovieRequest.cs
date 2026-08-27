
using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Requests
{
    public class CreateMovieRequest
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = "";
        [Required]
        public IFormFile Photo { get; set; } = null!;
        [Required]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = "";
        public string? Genre { get; set; } = "";
        [Required]
        [DataType(DataType.Date)]
        public DateOnly ReleaseDate { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public string genre{ get; set; } = "";
    }
}